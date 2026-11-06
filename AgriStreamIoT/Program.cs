using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using AgriStreamIoT.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => { o.LoginPath = "/Auth/Login"; o.AccessDeniedPath = "/Auth/AccessDenied"; o.Cookie.HttpOnly = true; o.Cookie.SecurePolicy = CookieSecurePolicy.Always; o.Cookie.SameSite = SameSiteMode.Lax; });
builder.Services.AddAuthorization();
builder.Services.AddSingleton(_ => new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"] ?? throw new InvalidOperationException("Missing Azure Storage connection string")));
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
var functionUrl = builder.Configuration["Functions:BaseUrl"] ?? throw new InvalidOperationException("Missing Functions:BaseUrl");
builder.Services.AddHttpClient<ITelemetryService, TelemetryService>(c => c.BaseAddress = new Uri(functionUrl));
builder.Services.AddHttpClient<IIrrigationService, IrrigationService>(c => c.BaseAddress = new Uri(functionUrl));
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}");
app.MapGet("/Error", () => Results.Problem("An unexpected error occurred."));
app.Run();
