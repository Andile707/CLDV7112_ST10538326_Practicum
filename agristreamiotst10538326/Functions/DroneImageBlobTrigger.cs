
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using agristreamiotst10538326.Models;

namespace AgriStreamIoT.Functions.Functions
{
    public class DroneImageBlobTrigger
    {
        private readonly ILogger<DroneImageBlobTrigger> _logger;

        public DroneImageBlobTrigger(
            ILogger<DroneImageBlobTrigger> logger)
        {
            _logger = logger;
        }

        [Function("DroneImageBlobTrigger")]
        public async Task Run(
            [BlobTrigger(
                "drone-images/{name}",
                Source = BlobTriggerSource.EventGrid,
                Connection = "AzureWebJobsStorage")]
            Stream blob,
            string name)
        {
            _logger.LogInformation(
                "Drone image detected: {BlobName}", name);

            try
            {
                string connectionString =
                    Environment.GetEnvironmentVariable(
                        "AzureWebJobsStorage")
                    ?? throw new InvalidOperationException(
                        "Azure Storage connection is missing.");

                // Connect to Azure Table Storage
                var tableClient = new TableClient(
                    connectionString,
                    "DroneUploadAudit");

                // Create the audit table if it does not exist
                await tableClient.CreateIfNotExistsAsync();

                // Create an audit record
                var audit = new DroneAuditEntity
                {
                    PartitionKey = "DroneUploads",
                    RowKey = Guid.NewGuid().ToString("N"),
                    BlobName = name,
                    SizeBytes = blob.CanSeek ? blob.Length : -1,
                    ProcessingStatus = "Uploaded"
                };

                // Save audit record to Azure Table Storage
                await tableClient.AddEntityAsync(audit);

                _logger.LogInformation(
                    "Drone image {BlobName} recorded in Azure Table Storage.",
                    name);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process drone image {BlobName}",
                    name);

                throw;
            }
        }
    }
}
