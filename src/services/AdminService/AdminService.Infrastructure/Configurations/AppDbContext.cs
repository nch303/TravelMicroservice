using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //public DbSet<AdvertisementPost> AdvertisementPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ==========================
            // AdvertisementPackage
            // ==========================
            //modelBuilder.Entity<AdvertisementPackage>(entity =>
            //{
            //    entity.HasKey(p => p.Id);
            //    entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            //    entity.Property(p => p.Description).HasMaxLength(1000);
            //    entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            //    entity.Property(p => p.DurationInDays).IsRequired();
            //    entity.Property(p => p.MaxPostCount).IsRequired();
            //    entity.Property(p => p.IsActive).IsRequired();
            //    entity.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()"); ;
            //});




            base.OnModelCreating(modelBuilder);
        }
    }
}
