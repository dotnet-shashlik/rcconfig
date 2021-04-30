using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.Application.Inputs
{
    public class CreateApplicationInput
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(255)] public string Desc { get; set; }

        public bool Enabled { get; set; }
    }
}