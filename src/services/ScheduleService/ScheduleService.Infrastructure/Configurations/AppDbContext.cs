using Microsoft.EntityFrameworkCore;
using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleParticipant> ScheduleParticipants { get; set; }
        public DbSet<ScheduleActivity> ScheduleActivities { get; set; }
        public DbSet<CheckedItem> CheckLists { get; set; }
        public DbSet<CheckedItemUser> CheckedItemUsers { get; set; }
        public DbSet<ScheduleMedia> ScheduleMedias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.SharedCode).IsRequired().HasMaxLength(10); // ID là chuỗi 10 số
                entity.Property(s => s.OwnerId).IsRequired();
                entity.Property(s => s.Title).IsRequired().HasMaxLength(255);
                entity.Property(s => s.StartLocation).HasMaxLength(255);
                entity.Property(s => s.Destination).HasMaxLength(255);
                entity.Property(s => s.ParticipantsCount).HasDefaultValue(0);
                entity.Property(s => s.IsShared).HasDefaultValue(false);
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(s => s.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<ScheduleParticipant>(entity =>
            {
                entity.HasKey(sp => sp.Id);
                entity.Property(sp => sp.UserId).IsRequired();
                entity.Property(sp => sp.Role)
                      .HasConversion<string>() // lưu enum dưới dạng string: "Owner", "Viewer", "Editor"
                      .HasMaxLength(50)
                      .IsRequired();
                entity.Property(sp => sp.JoineddAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(sp => sp.Status).HasMaxLength(50);

                // Thiết lập quan hệ 1-1 với Schedule
                entity.HasOne(sp => sp.Schedule)
                    .WithMany(s => s.ScheduleParticipants)
                    .HasForeignKey(sp => sp.ScheduleId)
                    .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Schedule bị xoá
            });

            modelBuilder.Entity<ScheduleActivity>(entity =>
            {
                entity.HasKey(sa => sa.Id);
                entity.Property(sa => sa.PlaceName).IsRequired().HasMaxLength(255);
                entity.Property(sa => sa.Location).HasMaxLength(500);
                entity.Property(sa => sa.Description).HasMaxLength(1000);
                entity.Property(sa => sa.CheckInTime).IsRequired();
                entity.Property(sa => sa.CheckOutTime).IsRequired();
                entity.Property(sa => sa.OrderIndex).IsRequired();
                entity.Property(sa => sa.IsDeleted).HasDefaultValue(false);

                // Thiết lập quan hệ 1-N với Schedule
                entity.HasOne(sa => sa.Schedule)
                    .WithMany(s => s.ScheduleActivities)
                    .HasForeignKey(sa => sa.ScheduleId)
                    .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Schedule bị xoá
            });

            modelBuilder.Entity<CheckedItem>(entity =>
            {
                entity.HasKey(cl => cl.Id);
                entity.Property(cl => cl.Name).IsRequired().HasMaxLength(255);

                // Thiết lập quan hệ 1-N với Schedule
                entity.HasOne(cl => cl.Schedule)
                    .WithMany(s => s.CheckLists)
                    .HasForeignKey(cl => cl.ScheduleId)
                    .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Schedule bị xoá
            });

            modelBuilder.Entity<CheckedItemUser>(entity =>
            {
                entity.HasKey(clu => clu.CheckedItemId);
                entity.Property(clu => clu.UserId).IsRequired();
                entity.Property(clu => clu.IsChecked).HasDefaultValue(false);
                entity.Property(clu => clu.CheckedAt);
            });

            modelBuilder.Entity<ScheduleMedia>(entity =>
            {
                entity.HasKey(sm => sm.Id);
                entity.Property(sm => sm.Url).IsRequired().HasMaxLength(1000);
                entity.Property(sm => sm.Description).HasMaxLength(1000);
                entity.Property(sm => sm.MediaType)
                      .HasConversion<string>() // lưu enum dưới dạng string: "Image", "Video", "Document"
                      .HasMaxLength(100)
                      .IsRequired();
                entity.Property(sm => sm.UploadedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(sm => sm.UploadedUserId).IsRequired();
                entity.Property(sm => sm.UploadMethod)
                      .HasConversion<string>() // lưu enum dưới dạng string: "Check-in", "Check-out"
                      .HasMaxLength(100)
                      .IsRequired();

                // Thiết lập quan hệ 1-N với Schedule
                entity.HasOne(sm => sm.Schedule)
                    .WithMany(s => s.ScheduleMedias)
                    .HasForeignKey(sm => sm.ScheduleId)
                    .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Schedule bị xoá
            });


            base.OnModelCreating(modelBuilder);

        }
    }
}

//dotnet ef migrations add Initial -o Migrations --project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.Infrastructure\ScheduleService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.API\ScheduleService.API.csproj"
//dotnet ef database update --project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.Infrastructure\ScheduleService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.API\ScheduleService.API.csproj"