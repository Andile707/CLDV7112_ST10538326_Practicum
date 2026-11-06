using System.Net.Http.Json;
using AgriStreamIoT.Models;
namespace AgriStreamIoT.Services;

public class IrrigationService(HttpClient client, IConfiguration config) : IIrrigationService
{
    public async Task TriggerAsync(IrrigationCommandModel command, CancellationToken ct = default)
    {
        var key = config["Functions:ApiKey"] ?? throw new InvalidOperationException("Missing Functions:ApiKey");
        using var req = new HttpRequestMessage(HttpMethod.Post, "api/irrigation") { Content = JsonContent.Create(command) };
        req.Headers.Add("x-functions-key", key);
        using var response = await client.SendAsync(req, ct);
        response.EnsureSuccessStatusCode();
    }
}
