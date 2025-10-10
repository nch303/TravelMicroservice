using AdvertisementService.Application.IServiceClients;
using AdvertisementService.Application.IServices;
using AdvertisementService.Application.ServiceClients;
using AdvertisementService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAdvertisementPackageService, AdvertisementPackageService>();
            services.AddScoped<IPartnerPackagePurchaseService, PartnerPackagePurchaseService>();
            services.AddScoped<IAdvertisementPostService, AdvertisementPostService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();


            // Cấu hình gọi API từ các service khác

            //Local
            var link = "https://localhost:5120";

            //Docker
            //var link = "http://apigateway:80";

            services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            return services;
        }
    }

}