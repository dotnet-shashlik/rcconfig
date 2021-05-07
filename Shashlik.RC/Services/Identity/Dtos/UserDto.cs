#nullable disable
using Microsoft.AspNetCore.Identity;
using Shashlik.AutoMapper;

namespace Shashlik.RC.Services.Identity.Dtos
{
    public class UserDto : IMapFrom<IdentityUser<int>>
    {
        public int Id { get; set; }

        public string UserName { get; set; }
    }
}