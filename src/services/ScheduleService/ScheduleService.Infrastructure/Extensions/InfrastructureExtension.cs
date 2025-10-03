using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScheduleService.Domain.IRepositories;
using ScheduleService.Infrastructure.Configurations;
using ScheduleService.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Đăng ký các repository
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IScheduleParticipantRepository, ScheduleParticipantRepository>();
            services.AddScoped<IScheduleActivityRepository, ScheduleActivityRepository>();
            services.AddScoped<ICheckItemParticipantRepository, CheckItemParticipantRepository>();
            services.AddScoped<ICheckedItemRepository, CheckedItemRepository>();
            services.AddScoped<IScheduleMediaRepository, ScheduleMediaRepository>();

            return services;
        }
    }

}
