using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Services.Application.Dtos;
using Shashlik.RC.Services.Application.Inputs;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.Application
{
    [Scoped]
    public class ApplicationService
    {
        public ApplicationService(RCDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        public async Task Create(CreateApplicationInput input)
        {
            var application = new Applications
            {
                Name = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate()
            };
            await DbContext.AddAsync(application);
            await DbContext.SaveChangesAsync();
        }

        public async Task Update(int id, UpdateApplicationInput input)
        {
            var application = await DbContext.FindAsync<Applications>(id);
            if (application is null)
                throw ResponseException.NotFound();
            application.Name = input.Name;
            application.Desc = input.Desc;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var application = await DbContext.FindAsync<Applications>(id);
            if (application is null)
                throw ResponseException.NotFound();
            DbContext.RemoveRange(DbContext.Files.Where(r => r.Environment.ApplicationId == id));
            DbContext.RemoveRange(DbContext.Environments.Where(r => r.ApplicationId == id));
            DbContext.Remove(application);
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<ApplicationDto>> List()
        {
            return await DbContext.Set<Applications>()
                .OrderBy(r => r.Id)
                .QueryTo<ApplicationDto>()
                .ToListAsync();
        }

        public async Task<ApplicationDto> Get(int id)
        {
            return await DbContext.Set<Applications>()
                .Where(r => r.Id == id)
                .QueryTo<ApplicationDto>()
                .FirstOrDefaultAsync();
        }
    }
}