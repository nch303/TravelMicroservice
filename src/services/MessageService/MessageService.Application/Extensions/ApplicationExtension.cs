using MessageService.Application.IServiceClients;
using MessageService.Application.IServices;
using MessageService.Application.ServiceClients;
using MessageService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IChatGroupService, ChatGroupService>();
            services.AddScoped<IChatParticipantService, ChatParticipantService>();
            services.AddScoped<IChatMessageService, ChatMessageService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Cấu hình gọi API từ các service khác

            //Local
            var link = "https://localhost:5120";

            //Docker
            //var link = "http://apigateway:80";

            services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            return services;
        }
    }
}
