using System.ComponentModel.DataAnnotations;

namespace Shashlik.RC.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "用户名/AppId不能为空")]
        [MaxLength(32, ErrorMessage = "用户名最多32个字符")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(32, ErrorMessage = "密码最多32个字符")]
        public string Password { get; set; }

        [Required(ErrorMessage = "验证码不能为空")]
        [MaxLength(6, ErrorMessage = "验证码为6个字符")]
        public string Code { get; set; }
    }
}
