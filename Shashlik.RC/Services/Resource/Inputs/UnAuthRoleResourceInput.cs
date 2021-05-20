using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.Permission.Inputs
{
    public class UnAuthRoleResourceInput
    {
        [Required, MaxLength(32)] public string Role { get; set; }
    }
}