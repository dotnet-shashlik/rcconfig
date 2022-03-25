using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Application;
using Shashlik.RC.Server.Services.Environment.Dtos;
using Shashlik.RC.Server.Services.Environment.Inputs;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Services.Environment;

[Scoped]
public class EnvironmentService
{
    public EnvironmentService(IFreeSql dbContext, ApplicationService applicationService)
    {
        DbContext = dbContext;
        ApplicationService = applicationService;
    }

    private IFreeSql DbContext { get; }
    private ApplicationService ApplicationService { get; }

    public async Task Create(string applicationName, CreateEnvironmentInput input)
    {
        var resourceId = $"{applicationName}/{input.Name}";
        if (await DbContext.Select<Environments>().AnyAsync(r => r.ResourceId == resourceId))
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
        await DbContext.Insert(environment).ExecuteAffrowsAsync();
    }

    public async Task Update(string resourceId, UpdateEnvironmentInput input)
    {
        await DbContext.Update<Environments>()
            .Where(r => r.ResourceId == resourceId)
            .Set(r => r.Desc, input.Desc)
            .Set(r => r.IpWhites, input.IpWhites)
            .ExecuteAffrowsAsync();
    }

    public async Task Delete(string resourceId)
    {
        await DbContext.Delete<Environments>()
            .Where(r => r.ResourceId == resourceId)
            .ExecuteAffrowsAsync();
    }

    public async Task<List<EnvironmentDto>> List(string? applicationName)
    {
        return await DbContext.Select<Environments>()
            .OrderBy(r => r.Id)
            .WhereIf(!applicationName.IsNullOrWhiteSpace(), r => r.Application.Name == applicationName)
            .ToListAsync<EnvironmentDto>();
    }

    public async Task<EnvironmentDto?> Get(string resourceId)
    {
        return await DbContext.Select<Environments>()
            .Where(r => r.ResourceId == resourceId)
            .FirstAsync<EnvironmentDto>();
    }

    public void UpdateVersion(string resourceId)
    {
        var time = DateTime.Now.GetLongDate();
        DbContext.Update<Environments>()
            .Where(r => r.ResourceId == resourceId)
            .Set(r => r.Version, time)
            .ExecuteAffrows();
    }
}