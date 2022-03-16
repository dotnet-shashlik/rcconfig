﻿using System.ComponentModel.DataAnnotations;
using Shashlik.RC.Server.Common;

#nullable disable
namespace Shashlik.RC.Server.Services.Application.Inputs
{
    public class CreateApplicationInput
    {
        [Required]
        [StringLength(32, MinimumLength = 1)]
        [RegularExpression(Constants.Regexs.Name)]
        public string Name { get; set; }

        [StringLength(255)] public string Desc { get; set; }
    }
}