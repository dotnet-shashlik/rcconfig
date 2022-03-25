using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Secret.Dtos;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Server.Services.Secret
{
    [Scoped]
    public class SecretService
    {
        public SecretService(IFreeSql dbContext)
        {
            DbContext = dbContext;
        }

        private IFreeSql DbContext { get; }

        public async Task<SecretDto?> GetBySecretId(string secretId)
        {
            return await DbContext.Select<Secrets>()
                .Where(r => r.SecretId == secretId)
                .FirstAsync<SecretDto>();
        }

        public async Task<List<SecretDto>> List(string userId)
        {
            return await DbContext.Select<Secrets>()
                .Where(r => r.UserId == userId)
                .ToListAsync<SecretDto>();
        }

        public async Task Create(string userId)
        {
            var secret = new Secrets
            {
                SecretId = Guid.NewGuid().ToString("n"),
                SecretKey = RandomKey(),
                CreateTime = DateTime.Now.GetLongDate(),
                UserId = userId,
            };

            await DbContext.Insert(secret).ExecuteAffrowsAsync();
        }

        public async Task Delete(string userId, string secretId)
        {
            var row = await DbContext.Delete<Secrets>()
                .Where(r => r.SecretId == secretId && r.UserId == userId)
                .ExecuteAffrowsAsync();
            if (row == 0)
                throw ResponseException.NotFound();
        }

        private string RandomKey()
        {
            return RandomHelper.RandomString(24);
        }
    }
}