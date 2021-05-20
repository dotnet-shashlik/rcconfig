﻿using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.Resource.Inputs
{
    public class UnAuthRoleResourceInput
    {
        [Required, MaxLength(255)] public string ResourceId { get; set; }
        [Required, MaxLength(32)] public string Role { get; set; }
    }
}