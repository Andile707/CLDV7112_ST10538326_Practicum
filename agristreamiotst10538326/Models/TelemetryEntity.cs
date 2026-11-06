
using Azure;
using Azure.Data.Tables;

namespace agristreamiotst10538326.Models
{
    public class TelemetryEntity : ITableEntity
    {
        // Required Azure Table Storage properties
        public string PartitionKey { get; set; } = string.Empty;

        public string RowKey { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        // Sensor information
        public string DeviceId { get; set; } = string.Empty;

        public string FieldId { get; set; } = string.Empty;

        public string SensorId { get; set; } = string.Empty;

        // Telemetry readings
        public double SoilMoisture { get; set; }

        public double Temperature { get; set; }

        // Time the sensor reading was recorded
        public DateTimeOffset TimestampUtc { get; set; }

        public DateTimeOffset RecordedAt { get; set; }
    }
}
