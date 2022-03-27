using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.EventBus;
using Shashlik.RC.Server.Services.Environment;
using Shashlik.RC.Server.Services.Environment.Dtos;

namespace Shashlik.RC.Server.Monitor;

/// <summary>
/// 资源数据监视器
/// </summary>
[Singleton]
public class ResourceMonitor : IEventHandler
{
    public ResourceMonitor(IServiceScopeFactory scopeFactory, CancelTokenSourceHolder cancelTokenSourceHolder)
    {
        ScopeFactory = scopeFactory;
        CancelTokenSourceHolder = cancelTokenSourceHolder;
    }

    public string Id = Guid.NewGuid().ToString();

    private IServiceScopeFactory ScopeFactory { get; }
    private CancelTokenSourceHolder CancelTokenSourceHolder { get; }


    /// <summary>
    /// 缓存的实时资源版本数据
    /// </summary>
    private static readonly ConcurrentDictionary<string, long> TimeVersions = new();

    /// <summary>
    /// 等待资源数据更新
    /// </summary>
    /// <param name="resourceId">资源id</param>
    /// <param name="version">客户端资源版本号</param>
    /// <param name="cancellationToken">客户端取消token</param>
    /// <returns></returns>
    public async Task<bool> WaitUpdate(string resourceId, long version, CancellationToken cancellationToken)
    {
        bool hold = false;
        if (TimeVersions.TryGetValue(resourceId, out var lasted))
        {
            if (lasted > version)
                return true;
            hold = true;
        }
        else if (!TimeVersions.ContainsKey(resourceId))
        {
            using var serviceScope = ScopeFactory.CreateScope();
            var environmentService = serviceScope.ServiceProvider.GetRequiredService<EnvironmentService>();
            var environmentDto = await environmentService.Get(resourceId).ConfigureAwait(false);
            if (environmentDto is null)
                return false;

            TimeVersions.AddOrUpdate(resourceId, environmentDto.Version, (_, _) => environmentDto.Version);
            if (environmentDto.Version > version)
                return true;
            hold = true;
        }

        if (hold)
        {
            var token = CancelTokenSourceHolder.Add(resourceId, cancellationToken);

            try
            {
                // 服务端最多等待30秒
                await Task.Delay(TimeSpan.FromSeconds(30), token).ConfigureAwait(false);
                if (TimeVersions.TryGetValue(resourceId, out var lasted1))
                    return lasted1 > version;
            }
            catch (OperationCanceledException)
            {
                if (TimeVersions.TryGetValue(resourceId, out var lasted1))
                    return lasted1 > version;
            }

            return false;
        }

        return false;
    }

    public void OnUpdate(string resourceId, long timeVersion)
    {
        TimeVersions.AddOrUpdate(resourceId, timeVersion, (_, _) => timeVersion);
        CancelTokenSourceHolder.Remove(resourceId);
    }

    public string EventName => Constants.Events.ResourceUpdated;

    public void Process(object? eventData)
    {
        var env = eventData as EnvironmentDto;
        TimeVersions.AddOrUpdate(env!.ResourceId, env.Version, (_, _) => env.Version);
        CancelTokenSourceHolder.Remove(env.ResourceId);
    }
}