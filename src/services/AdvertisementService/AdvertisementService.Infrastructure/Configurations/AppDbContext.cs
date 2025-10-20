using AdvertisementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AdvertisementPost> AdvertisementPosts { get; set; }
        public DbSet<AdvertisementPackage> AdvertisementPackages { get; set; }
        public DbSet<AdvertisementMedia> AdvertisementMedias { get; set; }
        public DbSet<PartnerPackagePurchase> PartnerPackagePurchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ==========================
            // AdvertisementPackage
            // ==========================
            modelBuilder.Entity<AdvertisementPackage>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Description).HasMaxLength(1000);
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(p => p.DurationInDays).IsRequired();
                entity.Property(p => p.MaxPostCount).IsRequired();
                entity.Property(p => p.IsActive).IsRequired();
                entity.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()"); ;
            });

            // ==========================
            // PartnerPackagePurchase
            // ==========================
            modelBuilder.Entity<PartnerPackagePurchase>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PartnerId).IsRequired();
                entity.Property(p => p.StartDate).IsRequired();
                entity.Property(p => p.EndDate).IsRequired();
                entity.Property(p => p.RemainingPostCount).IsRequired();
                entity.Property(p => p.PaymentTransactionId).IsRequired();
                entity.Property(p => p.Status).IsRequired();
                entity.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()"); ;

                entity.HasIndex(p => p.PartnerId);
                entity.HasIndex(p => p.PackageId);

                entity.HasOne(p => p.Package)
                      .WithMany(pkg => pkg.PartnerPackagePurchases)
                      .HasForeignKey(p => p.PackageId)
                      .OnDelete(DeleteBehavior.Restrict); // tránh xóa Package thì xóa luôn Purchase

            });

            // ==========================
            // AdvertisementPost
            // ==========================
            modelBuilder.Entity<AdvertisementPost>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.PartnerId).IsRequired();
                entity.Property(a => a.Title).IsRequired().HasMaxLength(255);
                entity.Property(a => a.Description).HasMaxLength(2000);
                entity.Property(a => a.PostedAt).IsRequired();
                entity.Property(a => a.ApprovedBy).IsRequired(false);
                entity.Property(a => a.ApprovedAt).IsRequired(false);
                entity.Property(a => a.Status).IsRequired();
                entity.Property(a => a.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()"); ;

                entity.HasIndex(a => a.PartnerId);
                entity.HasIndex(a => a.PackagePurchaseId);

                entity.HasOne(a => a.PackagePurchase)
                      .WithMany(p => p.AdvertisementPosts)
                      .HasForeignKey(a => a.PackagePurchaseId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            // ==========================
            // AdvertisementMedia
            // ==========================
            modelBuilder.Entity<AdvertisementMedia>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MediaUrl).IsRequired().HasMaxLength(500);
                entity.Property(m => m.MediaType).IsRequired();
                entity.Property(m => m.UploadedAt).IsRequired();

                entity.HasOne(m => m.AdvertisementPost)
                      .WithMany(p => p.MediaItems)
                      .HasForeignKey(m => m.AdvertisementPostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });




            base.OnModelCreating(modelBuilder);
        }
    }
}
