using AuthenticationDemo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDemo.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TrustedDevice> TrustedDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TotpSecret).IsRequired().HasMaxLength(200);
            });

            // TrustedDevice configuration
            modelBuilder.Entity<TrustedDevice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceToken).IsRequired().HasMaxLength(500);

                // DeviceToken + UserId kombinasyonu UNIQUE olmalı
                entity.HasIndex(e => new { e.DeviceToken, e.UserId }).IsUnique();

                entity.Property(e => e.DeviceName).HasMaxLength(200);
                entity.Property(e => e.IpAddress).HasMaxLength(50);

                // Foreign key relationship
                entity.HasOne(e => e.User)
                      .WithMany(u => u.TrustedDevices)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}