
using AgriStreamIoT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(
            IServiceProvider services,
            IConfiguration configuration)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var hasher = scope.ServiceProvider
                .GetRequiredService<IPasswordHasher<UserModel>>();

            // Read credentials from secure configuration
            var username = configuration["AdminSeed:Username"];
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            // Avoid creating duplicate Admin accounts
            bool exists = await db.Users
                .AnyAsync(u => u.Username == username);

            if (exists)
            {
                return;
            }

            var admin = new UserModel
            {
                Username = username,
                Role = "Admin"
            };

            // Hash the password before saving
            admin.PasswordHash =
                hasher.HashPassword(admin, password);

            db.Users.Add(admin);

            await db.SaveChangesAsync();
        }
    }
}
