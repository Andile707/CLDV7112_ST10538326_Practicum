
using AgriStreamIoT.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Database tables
        public DbSet<UserModel> Users { get; set; }

        public DbSet<DeviceModel> Devices { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users table
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Username)
                      .IsUnique();

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(80);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(20);
            });

            // Devices table
            modelBuilder.Entity<DeviceModel>(entity =>
            {
                entity.ToTable("Devices");

                entity.HasIndex(d => d.DeviceId)
                      .IsUnique();
            });
        }
    }
}
