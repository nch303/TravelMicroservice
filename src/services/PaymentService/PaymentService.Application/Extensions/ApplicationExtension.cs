using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.IServiceClients;
using PaymentService.Application.IServices;
using PaymentService.Application.ServiceClients;
using PaymentService.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITransactionService, TransactionService>();


            // Cấu hình gọi API từ các service khác

            //Local
            //var link = "https://localhost:5120";

            //Docker
            var link = "http://apigateway:80";

            services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri(link);
            });

            return services;
        }
    }
}
