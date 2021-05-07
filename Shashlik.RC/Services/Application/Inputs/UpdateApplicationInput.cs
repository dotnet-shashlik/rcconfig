using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.Application.Inputs
{
    public class UpdateApplicationInput
    {
        [StringLength(255)] public string Desc { get; set; }
    }
}