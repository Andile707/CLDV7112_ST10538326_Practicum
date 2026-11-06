namespace AgriStreamIoT.Models;

public class SensorReadingModel
{
    public string DeviceId { get; set; } = "";
    public string FieldId { get; set; } = "";
    public double SoilMoisture { get; set; }
    public double Temperature { get; set; }
    public DateTime TimestampUtc { get; set; }
}
