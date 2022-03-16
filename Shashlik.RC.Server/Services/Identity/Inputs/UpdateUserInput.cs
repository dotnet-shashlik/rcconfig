#nullable disable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shashlik.RC.Server.Services.Identity.Inputs
{
    public class UpdateUserInput
    {
        [Required, StringLength(32, MinimumLength = 1)]

        public string UserName { get; set; }

        [Required, MinLength(1)] public List<string> Roles { get; set; }

        [Required, StringLength(32, MinimumLength = 1)]
        public string NickName { get; set; }

        [StringLength(255)] public string Remark { get; set; }
    }
}