using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Identity;
using Shashlik.RC.Server.Services.Secret.Dtos;
using Shashlik.RC.Server.Services.Identity.Dtos;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;
using Z.EntityFramework.Plus;

namespace Shashlik.RC.Server.Services.Secret
{
    [Scoped]
    public class SecretService
    {
        public SecretService(RCDbContext dbContext, UserService userService)
        {
            DbContext = dbContext;
            UserService = userService;
        }

        private RCDbContext DbContext { get; }
        private UserService UserService { get; }

        public async Task<SecretDto?> GetBySecretId(string secretId)
        {
            return await DbContext.Set<Secrets>()
                .Where(r => r.SecretId == secretId)
                .QueryTo<SecretDto>()
                .FirstOrDefaultAsync();
        }

        public async Task<List<SecretDto>> List(string userId)
        {
            return await DbContext.Set<Secrets>()
                .Where(r => r.UserId == userId)
                .QueryTo<SecretDto>()
                .ToListAsync();
        }

        public async Task<SecretDto> Create(string userId)
        {
            var secret = new Secrets
            {
                SecretId = Guid.NewGuid().ToString("n"),
                SecretKey = RandomKey(userId),
                CreateTime = DateTime.Now.GetLongDate(),
                UserId = userId,
            };

            await DbContext.AddAsync(secret);
            await DbContext.SaveChangesAsync();

            return secret.MapTo<SecretDto>();
        }

        public async Task Delete(string userId, string secretId)
        {
            var row = await DbContext.Set<Secrets>()
                .Where(r => r.SecretId == secretId && r.UserId == userId)
                .DeleteAsync();
            if (row == 0)
                throw ResponseException.NotFound();
        }

        private string RandomKey(string resourceId)
        {
            var s = HashHelper.HMACSHA1(Guid.NewGuid().ToString(), resourceId);
            return s[4..s.Length];
        }
    }
}