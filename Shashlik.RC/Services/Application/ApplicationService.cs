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
            if (await DbContext.Set<Applications>().AnyAsync(r => r.Name == input.Name))
                throw ResponseException.ArgError("应用名称重复");
            var application = new Applications
            {
                Name = input.Name,
                ResourceId = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate()
            };
            await DbContext.AddAsync(application);
            await DbContext.SaveChangesAsync();
        }

        public async Task Update(string name, UpdateApplicationInput input)
        {
            var application = await DbContext.Set<Applications>().FirstOrDefaultAsync(r => r.Name == name);
            if (application is null)
                throw ResponseException.NotFound();
            application.Desc = input.Desc;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(string name)
        {
            var application = await DbContext.Set<Applications>().FirstOrDefaultAsync(r => r.Name == name);
            if (application is null)
                throw ResponseException.NotFound();
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

        public async Task<ApplicationDto> Get(string name)
        {
            return await DbContext.Set<Applications>()
                .Where(r => r.Name == name)
                .QueryTo<ApplicationDto>()
                .FirstOrDefaultAsync();
        }
    }
}