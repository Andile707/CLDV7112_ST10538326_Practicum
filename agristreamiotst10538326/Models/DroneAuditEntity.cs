
using Azure;
using Azure.Data.Tables;

namespace agristreamiotst10538326.Models
{
    public class DroneAuditEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "DroneUploads";

        public string RowKey { get; set; } =
            Guid.NewGuid().ToString("N");

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string BlobName { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        public string ProcessingStatus { get; set; } = "Uploaded";
    }
}
