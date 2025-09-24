using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.DateOfBirth);
                entity.Property(u => u.Address);
                entity.Property(u => u.AvatarUrl);
                entity.Property(u => u.Gender);
                entity.Property(u => u.PhoneNumber);
                entity.Property(u => u.Name).IsRequired();
            });

           
            base.OnModelCreating(modelBuilder);
        }
    }
}

//dotnet ef migrations add InitialCreate -o Migrations --project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.Infrastructure\UserService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.API\UserService.API.csproj"

//dotnet ef database update --project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.Infrastructure\UserService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.API\UserService.API.csproj"