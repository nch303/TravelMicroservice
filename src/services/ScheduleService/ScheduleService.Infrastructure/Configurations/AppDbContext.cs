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
        public DbSet<CheckedItem> CheckedItems { get; set; }
        public DbSet<CheckedItemParticipant> CheckedItemParticipants { get; set; }
        public DbSet<ScheduleMedia> ScheduleMedias { get; set; }
        public DbSet<ActivityAttendance> ActivityAttendances { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Schedule
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
                entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(50);
            });

            // ScheduleParticipant
            modelBuilder.Entity<ScheduleParticipant>(entity =>
            {
                entity.HasKey(sp => sp.Id);
                entity.Property(sp => sp.UserId).IsRequired();
                entity.Property(sp => sp.Role)
                      .HasConversion<string>() // lưu enum dưới dạng string: "Owner", "Viewer", "Editor"
                      .HasMaxLength(50)
                      .IsRequired();
                entity.Property(sp => sp.JoineddAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(sp => sp.Status).HasConversion<string>().HasMaxLength(50);

                // Thiết lập quan hệ 1-1 với Schedule
                entity.HasOne(sp => sp.Schedule)
                    .WithMany(s => s.ScheduleParticipants)
                    .HasForeignKey(sp => sp.ScheduleId)
                    .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Schedule bị xoá
            });

            // ScheduleActivity
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

            // CheckedItem (CheckList)
            modelBuilder.Entity<CheckedItem>(entity =>
            {
                entity.HasKey(cl => cl.Id);
                entity.Property(cl => cl.Name).IsRequired().HasMaxLength(255);
                entity.Property(cl => cl.IsDelete).HasDefaultValue(false);

                // Thiết lập quan hệ 1-N với Schedule
                entity.HasOne(cl => cl.Schedule)
                    .WithMany(s => s.CheckLists)
                    .HasForeignKey(cl => cl.ScheduleId)
                    .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Schedule bị xoá
            });

            // CheckedItemParticipant (bảng liên kết N-N giữa CheckedItem và ScheduleParticipant)
            modelBuilder.Entity<CheckedItemParticipant>(entity =>
            {
                entity.HasKey(cip => new { cip.CheckedItemId, cip.ScheduleParticipantId });

                entity.Property(cip => cip.IsChecked).HasDefaultValue(false);
                entity.Property(cip => cip.IsDeleted).HasDefaultValue(false);
                entity.Property(cip => cip.CheckedAt);

                entity.HasOne(cip => cip.CheckedItem)
                      .WithMany(ci => ci.CheckedItemParticipants)
                      .HasForeignKey(cip => cip.CheckedItemId)
                      .OnDelete(DeleteBehavior.Cascade); // giữ cascade ở đây

                entity.HasOne(cip => cip.ScheduleParticipant)
                      .WithMany(sp => sp.CheckedItemParticipants)
                      .HasForeignKey(cip => cip.ScheduleParticipantId)
                      .OnDelete(DeleteBehavior.Restrict); // hoặc NoAction
            });

            // ScheduleMedia
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
                    .OnDelete(DeleteBehavior.Restrict); // Xoá cascade nếu Schedule bị xoá

                // Thiết lập quan hệ N-1 với ScheduleActivity
                entity.HasOne(sm => sm.Activity)
                    .WithMany(sa => sa.ScheduleMedias)
                    .HasForeignKey(sm => sm.ActivityId)
                    .OnDelete(DeleteBehavior.Restrict); // Nếu ScheduleActivity bị xoá, giữ lại ScheduleMedia
            });

            // ActivityAttendance
            modelBuilder.Entity<ActivityAttendance>(entity =>
            {
                entity.HasKey(aa => aa.Id);
                entity.Property(aa => aa.CheckInTime).IsRequired(false);
                entity.Property(aa => aa.CheckOutTime).IsRequired(false);
                entity.Property(aa => aa.Status).HasConversion<string>().HasMaxLength(50);

                // Thiết lập quan hệ 1-N với ScheduleActivity
                entity.HasOne(aa => aa.Activity)
                    .WithMany(a => a.ActivityAttendances)
                    .HasForeignKey(aa => aa.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Thiết lập quan hệ 1-N với ScheduleParticipant
                entity.HasOne(aa => aa.Participant)
                   .WithMany(p => p.ActivityAttendances)
                   .HasForeignKey(aa => aa.ParticipantId)
                   .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SenderId).IsRequired();
                entity.Property(e => e.RecipientId).IsRequired(false);
                entity.Property(e => e.Title).HasMaxLength(100);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(100);

                // Thiết lập quan hệ 1-N với Schedule
                entity.HasOne(n => n.Schedule)
                      .WithMany(s => s.Notifications)
                      .HasForeignKey(n => n.ScheduleId)
                      .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Schedule bị xoá
            });

            // NotificationRecipient
            modelBuilder.Entity<NotificationRecipient>(entity =>
            {
                entity.HasKey(nr => nr.Id);
                entity.Property(nr => nr.IsRead).HasDefaultValue(false);
                entity.Property(nr => nr.ReadAt).HasDefaultValue(null).IsRequired(false);
                entity.Property(nr => nr.RecipientId);

                // Thiết lập quan hệ N-1 với Notification
                entity.HasOne(nr => nr.Notification)
                      .WithMany(n => n.NotificationRecipients)
                      .HasForeignKey(nr => nr.NotificationId)
                      .OnDelete(DeleteBehavior.Cascade); // Xoá cascade nếu Notification bị xoá
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}

//dotnet ef migrations add Initial -o Migrations --project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.Infrastructure\ScheduleService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.API\ScheduleService.API.csproj"
//dotnet ef database update --project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.Infrastructure\ScheduleService.Infrastructure.csproj" --startup-project "E:\TravelProject\TravelMicroservice\src\services\ScheduleService\ScheduleService.API\ScheduleService.API.csproj"