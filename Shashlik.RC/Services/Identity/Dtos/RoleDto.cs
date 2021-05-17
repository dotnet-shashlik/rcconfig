#nullable disable
using Microsoft.AspNetCore.Identity;
using Shashlik.AutoMapper;

namespace Shashlik.RC.Services.Identity.Dtos
{
    public class RoleDto : IMapFrom<IdentityRole<int>>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}