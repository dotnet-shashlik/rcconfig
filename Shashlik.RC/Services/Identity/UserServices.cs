using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Services.Identity.Dtos;

namespace Shashlik.RC.Services.Identity
{
    [Scoped]
    public class UserServices : UserManager<IdentityUser<int>>
    {
        public UserServices(IUserStore<IdentityUser<int>> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<IdentityUser<int>> passwordHasher, IEnumerable<IUserValidator<IdentityUser<int>>> userValidators,
            IEnumerable<IPasswordValidator<IdentityUser<int>>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<IdentityUser<int>>> logger) : base(store, optionsAccessor, passwordHasher, userValidators,
            passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<UserDto> Get(int userId)
        {
            return await Users.Where(r => r.Id == userId).QueryTo<UserDto>().FirstOrDefaultAsync();
        }

        public async Task<List<UserDto>> Get()
        {
            return await Users.QueryTo<UserDto>().ToListAsync();
        }
    }
}