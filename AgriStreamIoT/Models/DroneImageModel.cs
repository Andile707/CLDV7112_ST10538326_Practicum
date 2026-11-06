namespace AgriStreamIoT.Models;

public class DroneImageModel
{
    public IFormFile? Image { get; set; }
    public string FieldId { get; set; } = "";
}
