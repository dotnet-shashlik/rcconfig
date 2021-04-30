using System.ComponentModel.DataAnnotations;
using static Shashlik.RC.Enums;

namespace Shashlik.RC.Models
{
    public class ConfigAddOrUpdateModel
    {
        /// <summary>
        /// 配置id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 环境id
        /// </summary>
        public int EnvId { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 配置类型,json,xml,yaml,ini....
        /// </summary>
        public ConfigType Type { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        [RegularExpression(Consts.Regexs.LetterOrNumberOrUnderline, ErrorMessage = "配置名称只能由英文/数字/下划线组成")]
        [Required(ErrorMessage = "配置名称不能为空")]
        [MaxLength(32, ErrorMessage = "配置描述最大32个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        [MaxLength(512, ErrorMessage = "配置描述最大512个字符")]
        public string Desc { get; set; }
    }
}