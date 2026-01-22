using AuthenticationAPI.Application.DTOs;
using ECommerce.SharedLibrary.Responses;

namespace AuthenticationAPI.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> Register(AppUserDTO appUserDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUser(int userId);
    }
}
