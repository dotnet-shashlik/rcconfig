#nullable disable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shashlik.RC.Services.Identity.Inputs
{
    public class CreateUserInput
    {
        [Required, StringLength(32, MinimumLength = 1)]

        public string UserName { get; set; }

        [Required, StringLength(32)] public string Password { get; set; }

        [Required, StringLength(32)] public string ConfirmPassword { get; set; }

        [Required, MinLength(1)] public List<string> Roles { get; set; }
    }
}