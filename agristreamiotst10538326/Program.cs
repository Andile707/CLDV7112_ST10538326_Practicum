
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<
        Microsoft.Extensions.Configuration.IConfiguration>();

    var connectionString =
        configuration["AzureWebJobsStorage"];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "AzureWebJobsStorage is not configured.");
    }

    return new QueueServiceClient(connectionString);
});

builder.Build().Run();
