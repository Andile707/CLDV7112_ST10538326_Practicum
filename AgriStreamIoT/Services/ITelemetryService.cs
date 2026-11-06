using AgriStreamIoT.Models;
namespace AgriStreamIoT.Services;

public interface ITelemetryService { Task<List<SensorReadingModel>> LatestAsync(string fieldId, CancellationToken ct = default); }
