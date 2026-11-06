
using Azure;
using Azure.Data.Tables;

namespace agristreamiotst10538326.Models
{
    public class DroneAuditEntity : ITableEntity
    {
        // Required Azure Table Storage properties
        public string PartitionKey { get; set; } = string.Empty;

        public string RowKey { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        // Drone image information
        public string BlobName { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        public string Status { get; set; } = "Uploaded";

        public DateTimeOffset UploadedAt { get; set; }
    }
}
