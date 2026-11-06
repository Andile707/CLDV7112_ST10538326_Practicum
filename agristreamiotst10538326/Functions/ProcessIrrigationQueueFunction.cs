using System.Text.Json;
using AgriStreamIoT.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
namespace AgriStreamIoT.Functions.Functions;

public class ProcessIrrigationQueueFunction
{
    [Function("ProcessIrrigationQueueFunction")]
    public async Task Run([QueueTrigger("irrigation-commands", Connection = "AzureWebJobsStorage")] string message, FunctionContext context)
    {
        var logger = context.GetLogger("ProcessIrrigationQueueFunction");
        var command = JsonSerializer.Deserialize<IrrigationQueueMessage>(message) ?? throw new InvalidOperationException("Invalid queue message");
        var cs = Environment.GetEnvironmentVariable("SqlConnectionString") ?? throw new InvalidOperationException("Missing SQL connection");
        await using var connection = new SqlConnection(cs);
        await connection.OpenAsync();
        await using var sql = new SqlCommand("UPDATE Devices SET Status = @status, UpdatedUtc = @now WHERE DeviceId = @device AND FieldId = @field", connection);
        sql.Parameters.AddWithValue("@status", "IrrigationRequested");
        sql.Parameters.AddWithValue("@now", DateTime.UtcNow);
        sql.Parameters.AddWithValue("@device", command.DeviceId);
        sql.Parameters.AddWithValue("@field", command.FieldId);
        var changed = await sql.ExecuteNonQueryAsync();
        if (changed == 0) throw new InvalidOperationException($"Unknown device {command.DeviceId} in field {command.FieldId}");
        logger.LogInformation("Updated device {DeviceId} for command {CommandId}", command.DeviceId, command.CommandId);
    }
}
