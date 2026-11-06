namespace AgriStreamIoT.Models;

public class DashboardViewModel
{
    public List<SensorReadingModel> Readings { get; set; } = new();
    public List<DeviceModel> Devices { get; set; } = new();
    public string? Error { get; set; }
}
