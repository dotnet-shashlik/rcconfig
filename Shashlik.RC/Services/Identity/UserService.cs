using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Services.Identity.Dtos;
using Shashlik.RC.Services.Identity.Inputs;
using Shashlik.Response;

namespace Shashlik.RC.Services.Identity
{
    [Scoped]
    public class UserService : UserManager<IdentityUser<int>>
    {
        public UserService(IUserStore<IdentityUser<int>> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<IdentityUser<int>> passwordHasher, IEnumerable<IUserValidator<IdentityUser<int>>> userValidators,
            IEnumerable<IPasswordValidator<IdentityUser<int>>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<IdentityUser<int>>> logger, RCDbContext dbContext) : base(store, optionsAccessor,
            passwordHasher, userValidators,
            passwordValidators, keyNormalizer, errors, services, logger)
        {
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        public async Task<UserDetailDto?> Get(int userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            if (user is null)
                return null;
            var claims = await GetClaimsAsync(user);
            var roles = await GetRolesAsync(user);
            return new UserDetailDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Claims = claims ?? new List<Claim>(),
                Roles = roles ?? new string[0]
            };
        }

        public async Task<List<UserDto>> Get()
        {
            return await Users.QueryTo<UserDto>().ToListAsync();
        }

        public async Task CreateUser(CreateUserInput input)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync();
            var user = new IdentityUser<int>
            {
                UserName = input.UserName
            };
            var res = await CreateAsync(user, input.Password);
            if (!res.Succeeded)
            {
                await transaction.RollbackAsync();
                throw ResponseException.ArgError(res.ToString());
            }

            res = await AddToRolesAsync(user, input.Roles);
            if (!res.Succeeded)
            {
                await transaction.RollbackAsync();
                throw ResponseException.ArgError(res.ToString());
            }

            await transaction.CommitAsync();
        }
    }
}