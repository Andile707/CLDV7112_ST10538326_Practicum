using AgriStreamIoT.Functions.Models;
using agristreamiotst10538326.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
namespace AgriStreamIoT.Functions.Functions;

public class GetTelemetryFunction
{
    [Function("GetTelemetryFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "telemetry")] HttpRequestData req)
    {
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var fieldId = query["fieldId"];
        if (string.IsNullOrWhiteSpace(fieldId) || fieldId.Length > 100) { var bad = req.CreateResponse(HttpStatusCode.BadRequest); await bad.WriteStringAsync("Valid fieldId required."); return bad; }
        var connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? throw new InvalidOperationException("Missing storage connection");
        var table = new TableClient(connection, "SensorTelemetry");
        await table.CreateIfNotExistsAsync();
        var readings = new List<object>();
        await foreach (var row in table.QueryAsync<TelemetryEntity>(x => x.PartitionKey == fieldId, maxPerPage: 100))
        {
            readings.Add(new { fieldId = row.PartitionKey, deviceId = row.DeviceId, soilMoisture = row.SoilMoisture, temperature = row.Temperature, timestampUtc = row.TimestampUtc });
            if (readings.Count >= 50) break;
        }
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(readings);
        return response;
    }
}
