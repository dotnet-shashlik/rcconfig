using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.RC.Services.Environment;

namespace Shashlik.RC.EventBus;

public class EventQueue
{
    public static ConcurrentDictionary<string, long> TimeVersions = new();
    public static ConcurrentDictionary<string, ConcurrentBag<CancellationTokenSource>> Tokens = new();

    public static async Task<bool> Wait(string resourceId, long current, CancellationToken cancellationToken)
    {
        bool hold = false;
        if (TimeVersions.TryGetValue(resourceId, out var lasted))
        {
            if (lasted > current)
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

            TimeVersions.AddOrUpdate(resourceId, environmentDto.FileUpdateTime, (a, b) => environmentDto.FileUpdateTime);
            if (lasted > current)
                return true;
            hold = true;
        }

        if (hold)
        {
            //TODO: ..
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            cancellationToken.Register(cancellationTokenSource.Cancel);
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationTokenSource.Token);
            if (TimeVersions.TryGetValue(resourceId, out var lasted1))
                return lasted1 > current;

            return false;
        }

        return false;
    }
}