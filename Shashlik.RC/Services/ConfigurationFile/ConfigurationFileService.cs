using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Common;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Services.ConfigurationFile.Dtos;
using Shashlik.RC.Services.ConfigurationFile.Inputs;
using Shashlik.RC.Services.Environment;
using Shashlik.Response;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.ConfigurationFile
{
    [Scoped]
    public class ConfigurationFileService
    {
        public ConfigurationFileService(RCDbContext dbContext, EnvironmentService environmentService)
        {
            DbContext = dbContext;
            EnvironmentService = environmentService;
        }

        private RCDbContext DbContext { get; }
        private EnvironmentService EnvironmentService { get; }

        public async Task Create(string environmentName, CreateConfigurationFileInput input)
        {
            var environment = await EnvironmentService.Get(environmentName);
            if (environment is null)
                throw ResponseException.NotFound();

            var application = new ConfigurationFiles
            {
                Name = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate(),
                EnvironmentId = environment.Id,
                Content = input.Content,
                Type = input.Type
            };
            await DbContext.AddAsync(application);
            await DbContext.SaveChangesAsync();
        }

        public async Task Update(string environmentName, int id, UpdateConfigurationFileInput input)
        {
            var environment = await EnvironmentService.Get(environmentName);
            var configurationFile = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (configurationFile is null || environment!.Id != configurationFile.EnvironmentId)
                throw ResponseException.NotFound();
            configurationFile.Desc = input.Desc;
            configurationFile.Name = input.Name;
            configurationFile.Type = input.Type;
            configurationFile.Content = input.Content;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(string environmentName, int id)
        {
            var environment = await EnvironmentService.Get(environmentName);
            var configurationFile = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (configurationFile is null || environment!.Id != configurationFile.EnvironmentId)
                throw ResponseException.NotFound();
            DbContext.Remove(configurationFile);
            await DbContext.SaveChangesAsync();
        }

        public async Task<PageModel<ConfigurationFileDto>> List(string environmentName, PageInput pageInput)
        {
            return await DbContext.Set<ConfigurationFiles>()
                .Where(r => r.Environment.Name == environmentName)
                .OrderBy(r => r.Id)
                .QueryTo<ConfigurationFileDto>()
                .PageQuery(pageInput);
        }

        public async Task<ConfigurationFileDto> Get(string environmentName, int id)
        {
            return await DbContext.Set<ConfigurationFiles>()
                .Where(r => r.Id == id && r.Environment.Name == environmentName)
                .QueryTo<ConfigurationFileDto>()
                .FirstOrDefaultAsync();
        }
    }
}