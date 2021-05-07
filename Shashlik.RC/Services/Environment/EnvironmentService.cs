using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Services.Application;
using Shashlik.RC.Services.Environment.Dtos;
using Shashlik.RC.Services.Environment.Inputs;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.Environment
{
    [Scoped]
    public class EnvironmentService
    {
        public EnvironmentService(RCDbContext dbContext, ApplicationService applicationService)
        {
            DbContext = dbContext;
            ApplicationService = applicationService;
        }

        private RCDbContext DbContext { get; }
        private ApplicationService ApplicationService { get; }

        public async Task Create(string applicationName, CreateEnvironmentInput input)
        {
            var application = await ApplicationService.Get(applicationName);
            if (application is null)
                throw ResponseException.NotFound();

            var environment = new Environments
            {
                Name = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate(),
                IpWhites = input.IpWhites,
                ApplicationId = application.Id
            };
            await DbContext.AddAsync(environment);
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DBConcurrencyException)
            {
                throw ResponseException.ArgError("应用名称已存在");
            }
        }

        public async Task Update(string name, UpdateEnvironmentInput input)
        {
            var environment = await DbContext.Set<Environments>().FirstOrDefaultAsync(r => r.Name == name);
            if (environment is null)
                throw ResponseException.NotFound();
            environment.Desc = input.Desc;
            environment.IpWhites = input.IpWhites;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(string name)
        {
            var environment = await DbContext.Set<Environments>().FirstOrDefaultAsync(r => r.Name == name);
            if (environment is null)
                throw ResponseException.NotFound();
            DbContext.Remove(environment);
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<EnvironmentDto>> List()
        {
            return await DbContext.Set<Environments>()
                .OrderBy(r => r.Id)
                .QueryTo<EnvironmentDto>()
                .ToListAsync();
        }

        public async Task<EnvironmentDto?> Get(string name)
        {
            return await DbContext.Set<Environments>()
                .Where(r => r.Name == name)
                .QueryTo<EnvironmentDto>()
                .FirstOrDefaultAsync();
        }

        public async Task<EnvironmentDto?> GetBySecretId(string secretId)
        {
            var secret = await DbContext.Set<Secrets>()
                .Include(r => r.Environment)
                .Where(r => r.SecretId == secretId)
                .FirstOrDefaultAsync();

            return secret?.Environment.MapTo<EnvironmentDto>();
        }
    }
}