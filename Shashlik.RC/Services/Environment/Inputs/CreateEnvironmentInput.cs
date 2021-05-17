using System.ComponentModel.DataAnnotations;
using Shashlik.RC.Common;

#nullable disable
namespace Shashlik.RC.Services.Environment.Inputs
{
    public class CreateEnvironmentInput
    {
        [Required]
        [StringLength(32, MinimumLength = 1)]
        [RegularExpression(Constants.Regexs.Name)]
        public string Name { get; set; }

        [StringLength(255)] public string Desc { get; set; }

        /// <summary>
        /// ip白名单
        /// </summary>
        [StringLength(255)]
        public string IpWhites { get; set; }
    }
}