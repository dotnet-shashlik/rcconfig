using System.ComponentModel.DataAnnotations;
using Shashlik.RC.Utils;

namespace Shashlik.RC.Models
{
    public class EnvAddOrUpdateModel
    {
        /// <summary>
        /// 环境变量id,有就是修改,没有就是新增
        /// </summary>
        public int? Id { get; set; }

        [Required(ErrorMessage = "环境变量名称不能为空")]
        [MaxLength(32, ErrorMessage = "环境变量名称最多32个字符")]
        [RegularExpression(Consts.Regexs.LetterOrNumberOrUnderline, ErrorMessage = "环境变量名称只能由英文/数字/下划线组成")]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(32, ErrorMessage = "描述最多32个字符")]
        public string Desc { get; set; }

        public string IpWhites { get; set; }
    }
}
