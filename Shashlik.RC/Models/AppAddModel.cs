using System.ComponentModel.DataAnnotations;

namespace Shashlik.RC.Models
{
    public class AppAddModel
    {
        [Required(ErrorMessage = "App名称不能为空")]
        [MaxLength(32, ErrorMessage = "App名称最多32个字符")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "App描述最多500个字符")]
        public string Desc { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(16, ErrorMessage = "密码最多16位")]
        [MinLength(6, ErrorMessage = "密码最少6位")]
        public string Password { get; set; }
    }
}
