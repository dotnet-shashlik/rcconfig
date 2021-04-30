using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Dependency;

namespace Shashlik.RC.Services
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
    }
}