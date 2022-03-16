using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Server.Services.Identity.Inputs
{
    public class CreateRoleInput
    {
        [Required,StringLength(32)]
        public string Name { get; set; }
    }
}