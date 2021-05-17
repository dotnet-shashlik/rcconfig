using System.ComponentModel.DataAnnotations;
using Shashlik.RC.Common;

#nullable disable
namespace Shashlik.RC.Services.ConfigurationFile.Inputs
{
    public class CreateConfigurationFileInput
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 1)]
        [RegularExpression(Constants.Regexs.Name)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(255)]
        public string Desc { get; set; }

        /// <summary>
        /// 文件类型,yaml/json
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// 文件内容
        /// </summary>
        public string Content { get; set; }
    }
}