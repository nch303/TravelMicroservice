using AdvertisementService.Domain.IRepositories;
using AdvertisementService.Infrastructure.Configurations;
using AdvertisementService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAdvertisementPackageRepository, AdvertisementPackageRepository>();
            services.AddScoped<IPartnerPackagePurchaseRepository, PartnerPackagePurchaseRepository>();
            services.AddScoped<IAdvertisementPostRepository, AdvertisementPostRepository>();


            return services;
        }
    }
}
