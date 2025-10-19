using AdminService.Application.IServiceClients;
using AdminService.Application.ServiceClients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            // Cấu hình gọi API từ các service khác

            //Local
            //var link = "https://localhost:5120";

            //Docker
            var link = "http://apigateway:80";

            services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            services.AddHttpClient<IScheduleServiceClient, ScheduleServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            services.AddHttpClient<IAdvertisementServiceClient, AdvertisementServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            return services;
        }
    }

}
