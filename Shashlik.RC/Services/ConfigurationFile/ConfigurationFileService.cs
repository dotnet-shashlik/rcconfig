using System;
using System.Data;
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

        public async Task Create(string environmentResourceId, CreateConfigurationFileInput input)
        {
            if (await DbContext.Set<ConfigurationFiles>().AnyAsync(r => r.EnvironmentResourceId == environmentResourceId && r.Name == input.Name))
                throw ResponseException.ArgError("文件名称重复");

            var environment = await EnvironmentService.Get(environmentResourceId);
            if (environment is null)
                throw ResponseException.NotFound();

            var application = new ConfigurationFiles
            {
                Name = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate(),
                EnvironmentId = environment.Id,
                EnvironmentResourceId = environmentResourceId,
                Content = input.Content,
                Type = input.Type
            };
            await DbContext.AddAsync(application);
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DBConcurrencyException)
            {
                throw ResponseException.ArgError("文件名称重复");
            }
        }

        public async Task Update(string environmentResourceId, int id, UpdateConfigurationFileInput input)
        {
            var configurationFile = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (configurationFile is null || configurationFile.EnvironmentResourceId != environmentResourceId)
                throw ResponseException.NotFound();
            configurationFile.Desc = input.Desc;
            configurationFile.Name = input.Name;
            configurationFile.Type = input.Type;
            configurationFile.Content = input.Content;
            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(string environmentResourceId, int id)
        {
            var configurationFile = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (configurationFile is null || configurationFile.EnvironmentResourceId != environmentResourceId)
                throw ResponseException.NotFound();
            DbContext.Remove(configurationFile);
            await DbContext.SaveChangesAsync();
        }

        public async Task<PageModel<ConfigurationFileListDto>> List(string environmentResourceId, PageInput pageInput)
        {
            return await DbContext.Set<ConfigurationFiles>()
                .Where(r => r.EnvironmentResourceId == environmentResourceId)
                .OrderBy(r => r.Id)
                .QueryTo<ConfigurationFileListDto>()
                .PageQuery(pageInput);
        }

        public async Task<ConfigurationFileDto?> Get(string environmentResourceId, int id)
        {
            return await DbContext.Set<ConfigurationFiles>()
                .Where(r => r.Id == id && r.EnvironmentResourceId == environmentResourceId)
                .QueryTo<ConfigurationFileDto>()
                .FirstOrDefaultAsync();
        }
    }
}