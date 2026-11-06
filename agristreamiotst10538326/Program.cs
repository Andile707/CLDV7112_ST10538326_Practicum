
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using agristreamiotst10538326.Services;

var builder = FunctionsApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURE AZURE FUNCTIONS
// ==========================================

builder.ConfigureFunctionsWebApplication();

// ==========================================
// 2. CONFIGURE AZURE STORAGE
// ==========================================

var storageConnectionString =
    builder.Configuration["AzureWebJobsStorage"];

if (string.IsNullOrWhiteSpace(storageConnectionString))
{
    throw new InvalidOperationException(
        "AzureWebJobsStorage is not configured. " +
        "Check local.settings.json.");
}

// ==========================================
// 3. REGISTER AZURE QUEUE STORAGE
// ==========================================

builder.Services.AddSingleton(
    new QueueServiceClient(storageConnectionString));

// ==========================================
// 4. BUILD THE FUNCTIONS APPLICATION
// ==========================================

var app = builder.Build();

// ==========================================
// 5. SEED SAMPLE TELEMETRY DATA
// ==========================================

// Only seed data when explicitly enabled
// in the local development environment.

if (builder.Environment.IsDevelopment() &&
    builder.Configuration["SeedSampleTelemetry"] == "true")
{
    await TelemetrySeeder.SeedAsync(
        storageConnectionString);
}

// ==========================================
// 6. START AZURE FUNCTIONS
// ==========================================

app.Run();
