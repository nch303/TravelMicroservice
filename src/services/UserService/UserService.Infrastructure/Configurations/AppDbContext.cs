using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }

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

            modelBuilder.Entity<OtpVerification>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Email).IsRequired().HasMaxLength(255);
                entity.Property(o => o.OtpCode).IsRequired().HasMaxLength(10);
                entity.Property(o => o.ExpiresAt).IsRequired();
                entity.Property(o => o.Purpose).IsRequired().HasMaxLength(100);
                entity.Property(o => o.IsUsed).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

//dotnet ef migrations add otpVerification --project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.Infrastructure\UserService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.API\UserService.API.csproj"


//dotnet ef database update --project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.Infrastructure\UserService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\UserService\UserService.API\UserService.API.csproj"

