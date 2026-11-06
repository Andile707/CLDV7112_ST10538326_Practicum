using System.ComponentModel.DataAnnotations;
namespace AgriStreamIoT.Models;

public class IrrigationCommandModel
{
    [Required] public string DeviceId { get; set; } = "";
    [Required] public string FieldId { get; set; } = "";
}
