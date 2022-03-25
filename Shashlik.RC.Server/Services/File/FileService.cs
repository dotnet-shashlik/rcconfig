using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Server.Services.File.Inputs;
using Shashlik.RC.Server.Services.Environment;
using Shashlik.RC.Server.Services.File.Dtos;
using Shashlik.RC.Server.Services.Log;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Services.File;

[Scoped]
public class FileService
{
    public FileService(IFreeSql dbContext, EnvironmentService environmentService, LogService logService)
    {
        DbContext = dbContext;
        EnvironmentService = environmentService;
        LogService = logService;
    }

    private IFreeSql DbContext { get; }
    private EnvironmentService EnvironmentService { get; }
    private LogService LogService { get; }

    public async Task Create(int userId, string userName, string environmentResourceId, CreateConfigurationFileInput input)
    {
        if (await DbContext.Select<Files>()
                .AnyAsync(r => r.EnvironmentResourceId == environmentResourceId && r.Name == input.Name))
            throw ResponseException.ArgError("文件名称重复");

        var environment = await EnvironmentService.Get(environmentResourceId);
        if (environment is null)
            throw ResponseException.NotFound();

        var file = new Files
        {
            Name = input.Name,
            Desc = input.Desc,
            CreateTime = DateTime.Now.GetLongDate(),
            EnvironmentId = environment.Id,
            EnvironmentResourceId = environmentResourceId,
            Content = input.Content,
            Type = input.Type
        };
        DbContext.Transaction(() =>
        {
            DbContext.Insert(file).ExecuteAffrows();
            EnvironmentService.UpdateVersion(environmentResourceId);
            LogService.Add(
                userId,
                userName, LogType.Add,
                file.Id,
                $"{file.Name}.{file.Type}",
                file.EnvironmentResourceId,
                "",
                file.Content
            );
        });
    }

    public async Task Update(int userId, string userName, string environmentResourceId, int id, UpdateConfigurationFileInput input)
    {
        var file = await DbContext.Select<Files>(id).FirstAsync();
        if (file is null || file.EnvironmentResourceId != environmentResourceId)
            throw ResponseException.NotFound();
        if (await DbContext.Select<Files>()
                .AnyAsync(r => r.Id != file.Id && r.EnvironmentResourceId == environmentResourceId && r.Name == input.Name))
            throw ResponseException.ArgError("文件名称重复");
        var beforeContent = file.Content;

        DbContext.Transaction(() =>
        {
            DbContext.Update<Files>(id)
                .Set(r => r.Desc, input.Desc)
                .Set(r => r.Name, input.Name)
                .Set(r => r.Type, input.Type)
                .Set(r => r.Content, input.Content)
                .ExecuteAffrows();

            LogService.Add(
                userId,
                userName, LogType.Update,
                file.Id,
                $"{file.Name}.{file.Type}",
                file.EnvironmentResourceId,
                beforeContent,
                file.Content
            );
            EnvironmentService.UpdateVersion(environmentResourceId);
        });
    }

    public async Task Delete(int userId, string userName, string environmentResourceId, int id)
    {
        var file = await DbContext.Select<Files>(id).FirstAsync();
        if (file is null || file.EnvironmentResourceId != environmentResourceId)
            throw ResponseException.NotFound();

        DbContext.Transaction(() =>
        {
            DbContext.Delete<Files>(file);
            LogService.Add(
                userId,
                userName, LogType.Delete,
                file.Id,
                $"{file.Name}.{file.Type}",
                file.EnvironmentResourceId,
                file.Content,
                ""
            );
            EnvironmentService.UpdateVersion(environmentResourceId);
        });
    }

    public async Task<PageModel<ConfigurationFileListDto>> List(string environmentResourceId, PageInput pageInput)
    {
        return await DbContext.Select<Files>()
            .Where(r => r.EnvironmentResourceId == environmentResourceId)
            .OrderBy(r => r.Id)
            .Page<Files, ConfigurationFileListDto>(pageInput);
    }

    public async Task<ConfigurationFileDto?> Get(string environmentResourceId, int id)
    {
        return await DbContext.Select<Files>()
            .Where(r => r.Id == id && r.EnvironmentResourceId == environmentResourceId)
            .FirstAsync<ConfigurationFileDto>();
    }

    public async Task<List<ConfigurationFileDto>> All(string environmentResourceId)
    {
        return await DbContext.Select<Files>()
            .Where(r => r.EnvironmentResourceId == environmentResourceId)
            .ToListAsync<ConfigurationFileDto>();
    }
}