using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Gateway).HasMaxLength(100).IsRequired(false);
                entity.Property(t => t.TransactionDate).IsRequired(false);
                entity.Property(t => t.AccountNumber).HasMaxLength(100).IsRequired(false);
                entity.Property(t => t.SubAccount).HasMaxLength(250).IsRequired(false);
                entity.Property(t => t.AmountIn).HasColumnType("decimal(20,2)").HasDefaultValue(0);
                entity.Property(t => t.AmountOut).HasColumnType("decimal(20,2)").HasDefaultValue(0);
                entity.Property(t => t.Accumulated).HasColumnType("decimal(20,2)").HasDefaultValue(0);
                entity.Property(t => t.Code).HasMaxLength(250).IsRequired(false);
                entity.Property(t => t.TransactionContent).HasColumnType("text").IsRequired(false);
                entity.Property(t => t.ReferenceNumber).HasMaxLength(255).IsRequired(false);
                entity.Property(t => t.Body).HasColumnType("text").IsRequired(false);
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(t => t.Status).HasMaxLength(50).HasDefaultValue("Pending");

                // Quan hệ nếu có
                entity.Property(t => t.PackageId).IsRequired(false);
                entity.Property(t => t.UserId).IsRequired(false);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
