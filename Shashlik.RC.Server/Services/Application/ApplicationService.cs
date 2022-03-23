using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Application.Dtos;
using Shashlik.RC.Server.Services.Application.Inputs;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Services.Application;

[Scoped]
public class ApplicationService
{
    public ApplicationService(IFreeSql dbContext)
    {
        DbContext = dbContext;
    }

    private IFreeSql DbContext { get; }

    public async Task Create(CreateApplicationInput input)
    {
        if (await DbContext.Select<Applications>().AnyAsync(r => r.Name == input.Name))
            throw ResponseException.ArgError("应用名称重复");
        var application = new Applications
        {
            Name = input.Name,
            ResourceId = input.Name,
            Desc = input.Desc,
            CreateTime = DateTime.Now.GetLongDate()
        };
        await DbContext.Insert(application).ExecuteAffrowsAsync();
    }

    public async Task Update(string name, UpdateApplicationInput input)
    {
        await DbContext.Update<Applications>()
            .Where(r => r.Name == name)
            .Set(r => r.Desc, input.Desc)
            .ExecuteAffrowsAsync();
    }

    public async Task Delete(string name)
    {
        await DbContext.Delete<Applications>()
            .Where(r => r.Name == name)
            .ExecuteAffrowsAsync();
    }

    public async Task<List<ApplicationDto>> List()
    {
        return await DbContext.Select<Applications>()
            .OrderBy(r => r.Id)
            .ToListAsync<ApplicationDto>();
    }

    public async Task<ApplicationDto> Get(string name)
    {
        return await DbContext.Select<Applications>()
            .Where(r => r.Name == name)
            .FirstAsync<ApplicationDto>();
    }
}