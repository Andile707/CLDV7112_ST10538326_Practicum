
using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using AgriStreamIoT.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. MVC SERVICES
// ==========================================

builder.Services.AddControllersWithViews();

// ==========================================
// 2. SQL SERVER DATABASE
// ==========================================

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Missing DefaultConnection.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==========================================
// 3. PASSWORD HASHING
// ==========================================

builder.Services.AddScoped<
    IPasswordHasher<UserModel>,
    PasswordHasher<UserModel>>();

// ==========================================
// 4. COOKIE AUTHENTICATION
// ==========================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";

        options.AccessDeniedPath = "/Auth/AccessDenied";

        options.Cookie.HttpOnly = true;

        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;

        options.Cookie.SameSite =
            SameSiteMode.Lax;

        options.ExpireTimeSpan =
            TimeSpan.FromHours(2);

        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ==========================================
// 5. AZURE BLOB STORAGE
// ==========================================

var storageConnectionString =
    builder.Configuration["AzureStorage:ConnectionString"]
    ?? throw new InvalidOperationException(
        "Missing Azure Storage connection string.");

builder.Services.AddSingleton(
    new BlobServiceClient(storageConnectionString));

builder.Services.AddScoped<
    IBlobStorageService,
    BlobStorageService>();

// ==========================================
// 6. AZURE FUNCTIONS HTTP CLIENTS
// ==========================================

var functionUrl =
    builder.Configuration["Functions:BaseUrl"]
    ?? throw new InvalidOperationException(
        "Missing Functions:BaseUrl");

builder.Services.AddHttpClient<
    ITelemetryService,
    TelemetryService>(client =>
    {
        client.BaseAddress = new Uri(functionUrl);
    });

builder.Services.AddHttpClient<
    IIrrigationService,
    IrrigationService>(client =>
    {
        client.BaseAddress = new Uri(functionUrl);
    });

// ==========================================
// 7. BUILD APPLICATION
// ==========================================

var app = builder.Build();

// ==========================================
// 8. SEED INITIAL ADMIN ACCOUNT
// ==========================================

// Run only during local development.
// AdminSeeder reads credentials from User Secrets.
if (app.Environment.IsDevelopment())
{
    await AdminSeeder.SeedAdminAsync(
        app.Services,
        app.Configuration);
}

// ==========================================
// 9. ERROR HANDLING
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// ==========================================
// 10. HTTP REQUEST PIPELINE
// ==========================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// ==========================================
// 11. MVC ROUTING
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// ==========================================
// 12. ERROR ENDPOINT
// ==========================================

app.MapGet("/Error", () =>
    Results.Problem("An unexpected error occurred."));

// ==========================================
// 13. START APPLICATION
// ==========================================

await AdminSeeder.SeedAdminAsync(
    app.Services,
    app.Configuration);

app.Run();
