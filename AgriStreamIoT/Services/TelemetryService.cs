using System.Net.Http.Json;
using AgriStreamIoT.Models;
namespace AgriStreamIoT.Services;

public class TelemetryService(HttpClient client, IConfiguration config) : ITelemetryService
{
    public async Task<List<SensorReadingModel>> LatestAsync(string fieldId, CancellationToken ct = default)
    {
        var key = config["Functions:ApiKey"] ?? throw new InvalidOperationException("Missing Functions:ApiKey");
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/telemetry?fieldId={Uri.EscapeDataString(fieldId)}");
        req.Headers.Add("x-functions-key", key);
        using var response = await client.SendAsync(req, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<SensorReadingModel>>(cancellationToken: ct) ?? new();
    }
}
