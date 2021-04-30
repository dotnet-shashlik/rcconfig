using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel.Dependency;

namespace Shashlik.RC.Services
{
    [Scoped]
    public class RoleService : RoleManager<IdentityRole<int>>
    {
        public RoleService(IRoleStore<IdentityRole<int>> store, IEnumerable<IRoleValidator<IdentityRole<int>>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<IdentityRole<int>>> logger) : base(store,
            roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}