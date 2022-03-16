using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Server.Services.Identity.Dtos;

namespace Shashlik.RC.Server.Services.Identity
{
    [Scoped]
    public class RoleService : RoleManager<IdentityRole<int>>
    {
        public RoleService(IRoleStore<IdentityRole<int>> store, IEnumerable<IRoleValidator<IdentityRole<int>>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<IdentityRole<int>>> logger) : base(store,
            roleValidators, keyNormalizer, errors, logger)
        {
        }

        public async Task<List<RoleDto>> List()
        {
            return await Roles.QueryTo<RoleDto>().ToListAsync();
        }
    }
}