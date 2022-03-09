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
using Shashlik.RC.Filters;
using Shashlik.RC.Services.ConfigurationFile.Dtos;
using Shashlik.RC.Services.ConfigurationFile.Inputs;
using Shashlik.RC.Services.Environment;
using Shashlik.RC.Services.Log;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.ConfigurationFile
{
    [Scoped]
    public class FileService
    {
        public FileService(RCDbContext dbContext, EnvironmentService environmentService, LogService logService)
        {
            DbContext = dbContext;
            EnvironmentService = environmentService;
            LogService = logService;
        }

        private RCDbContext DbContext { get; }
        private EnvironmentService EnvironmentService { get; }
        private LogService LogService { get; }

        public async Task Create(int userId, string userName, string environmentResourceId, CreateConfigurationFileInput input)
        {
            if (await DbContext.Set<ConfigurationFiles>()
                    .AnyAsync(r => r.EnvironmentResourceId == environmentResourceId && r.Name == input.Name))
                throw ResponseException.ArgError("文件名称重复");

            var environment = await EnvironmentService.Get(environmentResourceId);
            if (environment is null)
                throw ResponseException.NotFound();

            var file = new ConfigurationFiles
            {
                Name = input.Name,
                Desc = input.Desc,
                CreateTime = DateTime.Now.GetLongDate(),
                EnvironmentId = environment.Id,
                EnvironmentResourceId = environmentResourceId,
                Content = input.Content,
                Type = input.Type
            };


            await using var transaction = await DbContext.Database.BeginTransactionAsync();
            try
            {
                await DbContext.AddAsync(file);
                await DbContext.SaveChangesAsync();
                await EnvironmentService.UpdateVersion(environmentResourceId);
                await LogService.Add(
                    userId,
                    userName, LogType.Add,
                    file.Id,
                    $"{file.Name}.{file.Type}",
                    file.EnvironmentResourceId,
                    "",
                    file.Content,
                    null,
                    true
                );
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw ResponseException.ArgError("文件名称重复");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(int userId, string userName, string environmentResourceId, int id, UpdateConfigurationFileInput input)
        {
            var file = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (file is null || file.EnvironmentResourceId != environmentResourceId)
                throw ResponseException.NotFound();
            if (await DbContext.Set<ConfigurationFiles>()
                    .AnyAsync(r => r.Id != file.Id && r.EnvironmentResourceId == environmentResourceId && r.Name == input.Name))
                throw ResponseException.ArgError("文件名称重复");
            var beforeContent = file.Content;
            file.Desc = input.Desc;
            file.Name = input.Name;
            file.Type = input.Type;
            file.Content = input.Content;

            await using var tran = await DbContext.Database.BeginTransactionAsync();
            try
            {
                await LogService.Add(
                    userId,
                    userName, LogType.Update,
                    file.Id,
                    $"{file.Name}.{file.Type}",
                    file.EnvironmentResourceId,
                    beforeContent,
                    file.Content
                );
                await DbContext.SaveChangesAsync();
                await EnvironmentService.UpdateVersion(environmentResourceId);
                await tran.CommitAsync();
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(int userId, string userName, string environmentResourceId, int id)
        {
            var file = await DbContext.FindAsync<ConfigurationFiles>(id);
            if (file is null || file.EnvironmentResourceId != environmentResourceId)
                throw ResponseException.NotFound();

            await using var tran = await DbContext.Database.BeginTransactionAsync();
            try
            {
                DbContext.Remove(file);
                await LogService.Add(
                    userId,
                    userName, LogType.Delete,
                    file.Id,
                    $"{file.Name}.{file.Type}",
                    file.EnvironmentResourceId,
                    file.Content,
                    ""
                );
                await DbContext.SaveChangesAsync();
                await EnvironmentService.UpdateVersion(environmentResourceId);
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
                throw;
            }
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

        public async Task<List<ConfigurationFileDto>> All(string environmentResourceId)
        {
            return await DbContext.Set<ConfigurationFiles>()
                .Where(r => r.EnvironmentResourceId == environmentResourceId)
                .QueryTo<ConfigurationFileDto>()
                .ToListAsync();
        }
    }
}