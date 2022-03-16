using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.RC.Server.Services.Environment;

namespace Shashlik.RC.Server.EventBus;

public class EventQueue
{
    public static readonly ConcurrentDictionary<string, long> TimeVersions = new();

    public static async Task<bool> Wait(string resourceId, long version, CancellationToken cancellationToken)
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
            using var serviceScope = GlobalKernelServiceProvider.KernelServiceProvider!.CreateScope();
            var environmentService = serviceScope.ServiceProvider.GetRequiredService<EnvironmentService>();
            var environmentDto = await environmentService.Get(resourceId);
            if (environmentDto is null)
                return false;

            TimeVersions.AddOrUpdate(resourceId, environmentDto.Version, (a, b) => environmentDto.Version);
            if (lasted > version)
                return true;
            hold = true;
        }

        if (hold)
        {
            var token = CancelTokenSourceHolder.Add(resourceId, cancellationToken);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), token);
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

    public static void OnUpdate(string resourceId, long timeVersion)
    {
        TimeVersions.AddOrUpdate(resourceId, timeVersion, (a, b) => timeVersion);
        CancelTokenSourceHolder.Remove(resourceId);
    }
}