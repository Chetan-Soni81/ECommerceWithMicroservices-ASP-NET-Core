using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using ECommerce.SharedLibrary.DependencyInjection;
using Microsoft.Extensions.Options;
using ApiGateway.Presentation.Middleware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCacheManager(o => o.WithDictionaryHandle());
JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build(); 

app.UseCors();
app.UseHttpsRedirection();
app.UseMiddleware<AttachSigatureToRequest>();
app.UseOcelot().Wait();

app.Run();