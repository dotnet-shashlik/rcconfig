using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.Permission.Inputs
{
    public class UnbindRoleResourceInput
    {
        [Required, MaxLength(32)] public string Role { get; set; }
    }
}