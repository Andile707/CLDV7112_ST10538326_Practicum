using AgriStreamIoT.Models;
using Microsoft.EntityFrameworkCore;
namespace AgriStreamIoT.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<DeviceModel> Devices => Set<DeviceModel>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserModel>().HasIndex(x => x.Username).IsUnique();
        builder.Entity<DeviceModel>().HasIndex(x => x.DeviceId).IsUnique();
    }
}
