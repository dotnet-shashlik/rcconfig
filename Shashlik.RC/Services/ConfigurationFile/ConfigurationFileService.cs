using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Services.ConfigurationFile.Dtos;
using Shashlik.RC.Services.ConfigurationFile.Inputs;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.ConfigurationFile
{
    [Scoped]
    public class ConfigurationFileService
    {
        public ConfigurationFileService(RCDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        public async Task Create(int environmentId, CreateConfigurationFileInput input)
        {
            var application = new ConfigurationFiles
            {
                Name = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate(),
                EnvironmentId = environmentId,
                Content = input.Content,
                Type = input.Type
            };
            await DbContext.AddAsync(application);
            await DbContext.SaveChangesAsync();
        }

        public async Task Update(int id, UpdateConfigurationFileInput input)
        {
            var configurationFile = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (configurationFile is null)
                throw ResponseException.NotFound();
            configurationFile.Desc = input.Desc;
            configurationFile.Name = input.Name;
            configurationFile.Type = input.Type;
            configurationFile.Content = input.Content;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var configurationFile = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (configurationFile is null)
                throw ResponseException.NotFound();
            DbContext.Remove(configurationFile);
            await DbContext.SaveChangesAsync();
        }

        public async Task<List<ConfigurationFileDto>> List()
        {
            return await DbContext.Set<ConfigurationFiles>()
                .OrderBy(r => r.Id)
                .QueryTo<ConfigurationFileDto>()
                .ToListAsync();
        }

        public async Task<ConfigurationFileDto> Get(int id)
        {
            return await DbContext.Set<ConfigurationFiles>()
                .Where(r => r.Id == id)
                .QueryTo<ConfigurationFileDto>()
                .FirstOrDefaultAsync();
        }
    }
}