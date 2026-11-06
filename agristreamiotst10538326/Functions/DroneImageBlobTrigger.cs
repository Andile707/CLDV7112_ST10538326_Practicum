using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using agristreamiotst10538326.Models;

namespace AgriStreamIoT.Functions.Functions;


public class DroneImageBlobTrigger
{
    [Function("DroneImageBlobTrigger")]
    public async Task Run([BlobTrigger("drone-images/{name}", Connection = "AzureWebJobsStorage")] Stream blob, string name, FunctionContext context)
    {
        var logger = context.GetLogger("DroneImageBlobTrigger");
        var connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? throw new InvalidOperationException("Missing storage connection");
        var table = new TableClient(connection, "DroneUploadAudit");
        await table.CreateIfNotExistsAsync();
        await table.AddEntityAsync(new DroneAuditEntity { BlobName = name, SizeBytes = blob.CanSeek ? blob.Length : -1 });
        logger.LogInformation("Recorded drone upload {BlobName}", name);
    }
}
