using MessageService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<MessageRead> MessageReads { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ChatGroup
            modelBuilder.Entity<ChatGroup>(entity =>
            {
                entity.HasKey(cg => cg.Id);

                entity.HasIndex(cg => cg.SharedCode)
                      .IsUnique(false)
                      .IsUnique();

                entity.Property(cg => cg.SharedExpired)
                      .IsRequired(false);

                entity.Property(cg => cg.Name)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(cg => cg.GroupType)
                      .HasConversion<string>() // Lưu enum dạng string
                      .HasMaxLength(50);

                entity.Property(cg => cg.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(cg => cg.ScheduleId)
                      .IsRequired(false); // có thể null

                entity.Property(cg => cg.UserId)
                      .IsRequired(); // người tạo group
            });

            // ChatParticipant
            modelBuilder.Entity<ChatParticipant>(entity =>
            {
                entity.HasKey(cp => cp.Id);

                entity.Property(cp => cp.UserId)
                      .IsRequired();

                entity.Property(cp => cp.Role)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                entity.Property(cp => cp.Status)
                      .HasConversion<string>()
                      .HasMaxLength(50);

                entity.Property(cp => cp.JoinedAt)
                        .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(cp => cp.LastSeenAt)
                      .IsRequired(false);

                entity.HasOne(cp => cp.ChatGroup)
                      .WithMany(cg => cg.Participants)
                      .HasForeignKey(cp => cp.ChatGroupId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatMessage
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(cm => cm.Id);

                entity.Property(cm => cm.MessageType)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.Property(cm => cm.Content)
                      .IsRequired()
                      .HasMaxLength(10000);

                entity.Property(cm => cm.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(cm => cm.EditAt)
                      .IsRequired(false);

                entity.Property(cm => cm.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                // Reply (self reference)
                entity.HasOne(cm => cm.ParentMessage)
                      .WithMany(pm => pm.Replies)
                      .HasForeignKey(cm => cm.ParentMessageId)
                      .OnDelete(DeleteBehavior.Restrict); // tránh multiple cascade


                entity.HasOne(cm => cm.Group)
                      .WithMany(cg => cg.Messages)
                      .HasForeignKey(cm => cm.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cm => cm.Sender)
                      .WithMany(cp => cp.Messages)
                      .HasForeignKey(cm => cm.SenderId)
                      .OnDelete(DeleteBehavior.Restrict); // tránh multiple cascade
            });

            // MessageReaction
            modelBuilder.Entity<MessageReaction>(entity =>
            {
                entity.HasKey(mr => mr.Id);

                entity.Property(mr => mr.ReactionType)
                      .HasMaxLength(50);

                entity.Property(mr => mr.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(mr => mr.IsDeleted)
                      .HasDefaultValue(false);

                entity.HasOne(mr => mr.Participant)
                      .WithMany(u => u.Reactions)
                      .HasForeignKey(mr => mr.ParticipantId)
                      .OnDelete(DeleteBehavior.Restrict); // tránh multiple cascade

                entity.HasOne(mr => mr.Message)
                      .WithMany(cm => cm.Reactions)
                      .HasForeignKey(mr => mr.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Unique: 1 user chỉ có 1 reaction cho 1 message
                entity.HasIndex(mr => new { mr.ParticipantId, mr.MessageId })
                      .IsUnique();
            });

            // MessageRead
            modelBuilder.Entity<MessageRead>(entity =>
            {
                entity.HasKey(mr => mr.Id);

                entity.Property(mr => mr.ReadAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(mr => mr.Reader)
                      .WithMany(u => u.Reads)
                      .HasForeignKey(mr => mr.ReaderId)
                      .OnDelete(DeleteBehavior.Restrict); // tránh multiple cascade

                entity.HasOne(mr => mr.Message)
                      .WithMany(cm => cm.Reads)
                      .HasForeignKey(mr => mr.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.Property(n => n.UserId)
                        .IsRequired();

                entity.Property(n => n.ChatGroupId)
                    .IsRequired(false);

                entity.Property(n => n.ChatMessageId)
                    .IsRequired(false);

                entity.Property(n => n.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(n => n.Content)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(n => n.IsRead)
                    .HasDefaultValue(false);

                entity.Property(n => n.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(n => n.ChatGroup)
                    .WithMany(cg => cg.Notifications)
                    .HasForeignKey(n => n.ChatGroupId)
                    .OnDelete(DeleteBehavior.Restrict); // nếu group bị xóa, giữ lại thông báo

                entity.HasOne(n => n.ChatMessage)
                    .WithMany(cm => cm.Notifications)
                    .HasForeignKey(n => n.ChatMessageId)
                    .OnDelete(DeleteBehavior.Restrict); // nếu message bị xóa, giữ lại thông báo
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
