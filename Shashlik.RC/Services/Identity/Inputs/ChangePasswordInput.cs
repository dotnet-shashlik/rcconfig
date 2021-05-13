﻿using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.Identity.Inputs
{
    public class ChangePasswordInput
    {
        [Required, StringLength(32)] public string OldPassword { get; set; }
        [Required, StringLength(32)] public string NewPassword { get; set; }
        [Required, StringLength(32)] public string ConfirmPassword { get; set; }
    }
}