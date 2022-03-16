using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Server.Services.Resource.Inputs
{
    public class AuthRoleResourceInput
    {
        [Required, MaxLength(255)] public string ResourceId { get; set; }
        [Required, MaxLength(32)] public string Role { get; set; }

        public PermissionAction Action { get; set; }
    }
}