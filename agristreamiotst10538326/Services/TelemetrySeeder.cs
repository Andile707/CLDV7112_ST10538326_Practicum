
using agristreamiotst10538326.Models;
using Azure.Data.Tables;

namespace agristreamiotst10538326.Services
{
    public static class TelemetrySeeder
    {
        public static async Task SeedAsync(
            string connectionString)
        {
            var table = new TableClient(
                connectionString,
                "SensorTelemetry");

            await table.CreateIfNotExistsAsync();

            // Avoid inserting duplicate sample data
            var existing = table.QueryAsync<TelemetryEntity>(
                x => x.PartitionKey == "field-1",
                maxPerPage: 1);

            await foreach (var reading in existing)
            {
                // Field already contains telemetry
                return;
            }

            var now = DateTimeOffset.UtcNow;

            var readings = new List<TelemetryEntity>
            {
                new TelemetryEntity
                {
                    PartitionKey = "field-1",
                    RowKey = Guid.NewGuid().ToString("N"),
                    FieldId = "field-1",
                    DeviceId = "sensor-001",
                    SensorId = "sensor-001",
                    SoilMoisture = 35.0,
                    Temperature = 26.0,
                    TimestampUtc = now,
                    RecordedAt = now
                },

                new TelemetryEntity
                {
                    PartitionKey = "field-1",
                    RowKey = Guid.NewGuid().ToString("N"),
                    FieldId = "field-1",
                    DeviceId = "sensor-002",
                    SensorId = "sensor-002",
                    SoilMoisture = 42.0,
                    Temperature = 28.0,
                    TimestampUtc = now,
                    RecordedAt = now
                },

                new TelemetryEntity
                {
                    PartitionKey = "field-1",
                    RowKey = Guid.NewGuid().ToString("N"),
                    FieldId = "field-1",
                    DeviceId = "sensor-003",
                    SensorId = "sensor-003",
                    SoilMoisture = 29.0,
                    Temperature = 30.0,
                    TimestampUtc = now,
                    RecordedAt = now
                }
            };

            foreach (var reading in readings)
            {
                await table.AddEntityAsync(reading);
            }
        }
    }
}
