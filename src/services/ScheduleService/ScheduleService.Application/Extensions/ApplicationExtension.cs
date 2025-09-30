using Microsoft.Extensions.DependencyInjection;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Application.ServiceClients;
using ScheduleService.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IScheduleService, SchedulesService>();
            services.AddScoped<IScheduleParticipantService, ScheduleParticipantService>();
            services.AddScoped<IScheduleActivityService, ScheduleActivityService>();
            services.AddScoped<ICheckedItemService, CheckedItemService>();

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
