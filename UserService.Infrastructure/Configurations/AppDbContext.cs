using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace Infrastructure.Configurations
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
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.IsActive).IsRequired();

                // Unique constraint cho email
                entity.HasIndex(u => u.Email).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

//dotnet ef migrations add InitialCreate --project E:\TravelProject\TravelMicroservice\UserService.Infrastructure\UserService.Infrastructure.csproj --startup-project E:\TravelProject\TravelMicroservice\UserService.API\UserService.API.csproj
//dotnet ef database update --project E:\TravelProject\TravelMicroservice\UserService.Infrastructure\UserService.Infrastructure.csproj --startup-project E:\TravelProject\TravelMicroservice\UserService.API\UserService.API.csproj