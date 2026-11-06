using System.ComponentModel.DataAnnotations;
namespace AgriStreamIoT.Models;

public class UserModel
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string Username { get; set; } = "";
    [Required] public string PasswordHash { get; set; } = "";
    [Required, MaxLength(20)] public string Role { get; set; } = "Farmer";
}
