using AgriStreamIoT.Models;
namespace AgriStreamIoT.Services;

public interface IIrrigationService { Task TriggerAsync(IrrigationCommandModel command, CancellationToken ct = default); }
