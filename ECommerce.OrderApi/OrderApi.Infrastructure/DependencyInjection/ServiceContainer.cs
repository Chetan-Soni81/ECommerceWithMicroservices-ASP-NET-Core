using ECommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add Database Connectivity
            // Add Authentication Scheme
            SharedServiceContainer.AddSharedServices<OrderDbContext>(services, config, config["MySerilog:FileName"]!);

            // Create Dependency Injection
            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register Middleware such as
            // Global Exception -> handle external errors
            // ListenToApiGateway -> block all outsider calls

            SharedServiceContainer.UseSharedServices(app);
            return app;
        }
    }
}
