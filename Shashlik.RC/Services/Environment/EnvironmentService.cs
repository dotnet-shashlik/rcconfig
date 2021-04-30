using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Services.Environment.Dtos;
using Shashlik.RC.Services.Environment.Inputs;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.Environment
{
    [Scoped]
    public class EnvironmentService
    {
        public EnvironmentService(RCDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        public async Task Create(int applicationId, CreateEnvironmentInput input)
        {
            var application = new Environments
            {
                Name = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate(),
                IpWhites = input.IpWhites,
                ApplicationId = applicationId
            };
            await DbContext.AddAsync(application);
            await DbContext.SaveChangesAsync();
        }

        public async Task Update(int id, UpdateEnvironmentInput input)
        {
            var environment = await DbContext.FindAsync<Environments>(id);
            if (environment is null)
                throw ResponseException.NotFound();
            environment.Desc = input.Desc;
            environment.Name = input.Name;
            environment.IpWhites = input.IpWhites;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var environment = await DbContext.FindAsync<Environments>(id);
            if (environment is null)
                throw ResponseException.NotFound();
            DbContext.RemoveRange(DbContext.Files.Where(r => r.EnvironmentId == id));
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

        public async Task<EnvironmentDto> Get(int id)
        {
            return await DbContext.Set<Environments>()
                .Where(r => r.Id == id)
                .QueryTo<EnvironmentDto>()
                .FirstOrDefaultAsync();
        }
    }
}