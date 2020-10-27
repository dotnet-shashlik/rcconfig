using System.ComponentModel.DataAnnotations;

namespace Shashlik.RC.Models
{
    public class ChangePasswordInput
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required(ErrorMessage = "旧密码不能为空")]
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "新密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码6-20位")]
        public string Password { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码6-20位")]
        public string ConfirmPassword { get; set; }
    }
}
