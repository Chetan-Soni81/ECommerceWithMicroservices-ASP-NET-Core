using AuthenticationAPI.Application.DTOs;
using AuthenticationAPI.Application.Interfaces;
using AuthenticationAPI.Domain.Entities;
using AuthenticationAPI.Infrastructure.Data;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationAPI.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user is not null ? new GetUserDTO(user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!) : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser is null) 
                return new Response(Flag: false, Message: "Invalid credentials");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (!verifyPassword) 
                return new Response(Flag: false, Message: "Invalid credentials");

            string token = GenerateToken(getUser);

            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            // Token generation logic here (e.g., using JWT)
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            if(!string.IsNullOrEmpty(user.Role) || Equals("string", user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role!));
            }

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials
            );

            // Replace with your actual secret key
            return new JwtSecurityTokenHandler().WriteToken(token); // Placeholder
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var getUser = await GetUserByEmail(appUserDTO.Email);
            if (getUser is not null) 
                return new Response(Flag: false, Message: "User with this email already exists");

            var result = context.Users.Add(new AppUser() {
                Name = appUserDTO.Name,
                TelephoneNumber= appUserDTO.TelephoneNumber,
                Email = appUserDTO.Email,
                Address = appUserDTO.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                Role = appUserDTO.Role
            });

            await context.SaveChangesAsync();

            return result.Entity.Id > 0 ? new Response(true, "User registered Successfully")
                : new Response(false, "Invalid data provided");
        }
    }
}
