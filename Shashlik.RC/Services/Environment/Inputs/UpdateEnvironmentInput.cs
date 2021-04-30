using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.Environment.Inputs
{
    public class UpdateEnvironmentInput
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(255)] public string Desc { get; set; }

        /// <summary>
        /// ip白名单
        /// </summary>
        [StringLength(255)]
        public string IpWhites { get; set; }
    }
}