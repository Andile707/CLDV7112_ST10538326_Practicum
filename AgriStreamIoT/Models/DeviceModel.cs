using System.ComponentModel.DataAnnotations;
namespace AgriStreamIoT.Models;

public class DeviceModel
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string DeviceId { get; set; } = "";
    [Required, MaxLength(100)] public string FieldId { get; set; } = "";
    [MaxLength(40)] public string Status { get; set; } = "Idle";
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
