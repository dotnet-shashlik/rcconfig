using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<IdentityRole<int>>> logger,
            IFreeSql dbContext) : base(store,
            roleValidators, keyNormalizer, errors, logger)
        {
            DbContext = dbContext;
        }

        private IFreeSql DbContext { get; }

        public async Task<List<RoleDto>> List()
        {
            return await DbContext.Select<IdentityRole<int>>().ToListAsync<RoleDto>();
        }
    }
}