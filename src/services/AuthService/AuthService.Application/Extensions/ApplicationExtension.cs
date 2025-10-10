using AuthService.Application.IServiceClients;
using AuthService.Application.IServices;
using AuthService.Application.ServiceClients;
using AuthService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthServices>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            // Cấu hình gọi API từ các service khác

            //Local
            var link = "https://localhost:5120";

            //Docker
            //var link = "http://apigateway:80";

            services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            return services;
        }
    }

}
