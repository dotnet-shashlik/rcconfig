#nullable disable
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Shashlik.AutoMapper;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Services.Identity.Dtos
{
    public class UserDto : IMapFrom<IdentityUser<int>>
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public List<string> Roles { get; set; }

        public string RolesStr => Roles.Join(",");
    }
}