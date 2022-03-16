using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Data;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Identity.Dtos;
using Shashlik.RC.Server.Services.Identity.Inputs;
using Z.EntityFramework.Plus;

namespace Shashlik.RC.Server.Services.Identity
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
            var list = await (from user in DbContext.Users
                    join userRole in DbContext.UserRoles on user.Id equals userRole.UserId
                    join role in DbContext.Roles on userRole.RoleId equals role.Id
                    select new
                    {
                        user.Id, user.UserName,
                        RoleName = role.Name
                    }
                )
                .ToListAsync();

            return list.GroupBy(r => new {r.Id, r.UserName})
                .Select(r => new UserDto
                {
                    Id = r.Key.Id,
                    UserName = r.Key.UserName,
                    Roles = r.Select(i => i.RoleName).ToList()
                })
                .ToList();
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

            res = await AddClaimsAsync(user, new List<Claim>()
            {
                new Claim(Constants.UserClaimTypes.NickName, input.NickName),
                new Claim(Constants.UserClaimTypes.Remark, input.Remark)
            });
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

        public async Task Update(int userId, UpdateUserInput input)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync();
            var user = await FindByIdAsync(userId.ToString());
            if (user is null)
            {
                await transaction.RollbackAsync();
                throw ResponseException.NotFound();
            }

            user.UserName = input.UserName;

            #region claim

            var res = await AddOrUpdateClaim(user, new Claim(Constants.UserClaimTypes.NickName, input.NickName));
            if (!res.Succeeded)
            {
                await transaction.RollbackAsync();
                throw ResponseException.ArgError(res.ToString());
            }

            res = await AddOrUpdateClaim(user, new Claim(Constants.UserClaimTypes.Remark, input.Remark));
            if (!res.Succeeded)
            {
                await transaction.RollbackAsync();
                throw ResponseException.ArgError(res.ToString());
            }

            #endregion

            #region roles

            var roles = await GetRolesAsync(user);
            if (!roles.IsNullOrEmpty())
            {
                res = await RemoveFromRolesAsync(user, roles);
                if (!res.Succeeded)
                {
                    await transaction.RollbackAsync();
                    throw ResponseException.ArgError(res.ToString());
                }
            }

            if (!input.Roles.IsNullOrEmpty())
            {
                res = await AddToRolesAsync(user, input.Roles);
                if (!res.Succeeded)
                {
                    await transaction.RollbackAsync();
                    throw ResponseException.ArgError(res.ToString());
                }
            }

            #endregion

            await transaction.CommitAsync();
        }

        public async Task<IdentityResult> AddOrUpdateClaim(IdentityUser<int> user, Claim claim)
        {
            var rows = await DbContext.UserClaims
                .Where(r => r.UserId == user.Id && r.ClaimType == claim.Type)
                .UpdateAsync(r => new IdentityUserClaim<int> {ClaimValue = claim.Value});
            if (rows == 0)
                return await AddClaimAsync(user, claim);

            return IdentityResult.Success;
        }
    }
}