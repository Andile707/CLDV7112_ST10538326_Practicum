namespace AgriStreamIoT.Functions.Models;

public class IrrigationQueueMessage
{
    public string CommandId { get; set; } = Guid.NewGuid().ToString("N");
    public string DeviceId { get; set; } = "";
    public string FieldId { get; set; } = "";
    public DateTime RequestedUtc { get; set; } = DateTime.UtcNow;
}
