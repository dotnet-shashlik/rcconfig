using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Application;
using Shashlik.RC.Services.Environment.Dtos;
using Shashlik.RC.Services.Environment.Inputs;
using Shashlik.Utils.Extensions;
using Z.EntityFramework.Plus;

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
            var resourceId = $"{applicationName}/{input.Name}";
            if (await DbContext.Set<Environments>().AnyAsync(r => r.ResourceId == resourceId))
                throw ResponseException.ArgError("环境名称重复");

            var application = await ApplicationService.Get(applicationName);
            if (application is null)
                throw ResponseException.NotFound();

            var environment = new Environments
            {
                Name = input.Name,
                ResourceId = resourceId,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate(),
                IpWhites = input.IpWhites,
                ApplicationId = application.Id
            };
            await DbContext.AddAsync(environment);
            await DbContext.SaveChangesAsync();
        }

        public async Task Update(string resourceId, UpdateEnvironmentInput input)
        {
            var environment = await DbContext.Set<Environments>()
                .FirstOrDefaultAsync(r => r.ResourceId == resourceId);
            if (environment is null)
                throw ResponseException.NotFound();
            environment.Desc = input.Desc;
            environment.IpWhites = input.IpWhites;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(string resourceId)
        {
            var environment = await DbContext.Set<Environments>()
                .FirstOrDefaultAsync(r => r.ResourceId == resourceId);
            if (environment is null)
                throw ResponseException.NotFound();
            DbContext.Remove(environment);
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<EnvironmentDto>> List(string? applicationName)
        {
            return await DbContext.Set<Environments>()
                .OrderBy(r => r.Id)
                .WhereIf(!applicationName.IsNullOrWhiteSpace(), r => r.Application.Name == applicationName)
                .QueryTo<EnvironmentDto>()
                .ToListAsync();
        }

        public async Task<EnvironmentDto?> Get(string resourceId)
        {
            return await DbContext.Set<Environments>()
                .Where(r => r.ResourceId == resourceId)
                .QueryTo<EnvironmentDto>()
                .FirstOrDefaultAsync();
        }

        public async Task UpdateVersion(string resourceId)
        {
            var time = DateTime.Now.GetLongDate();
            await DbContext.Set<Environments>()
                .Where(r => r.ResourceId == resourceId)
                .UpdateAsync(r => new Environments { Version = time });
        }
    }
}