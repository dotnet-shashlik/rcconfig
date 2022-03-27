using System.Collections.Concurrent;
using System.Threading;
using Shashlik.Kernel.Dependency;

namespace Shashlik.RC.Server.Monitor;

[Singleton]
public class CancelTokenSourceHolder
{
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<CancellationTokenSource, CancellationTokenSource>>
        Holder = new();

    public CancellationToken Add(string resourceId, CancellationToken requestToken)
    {
        var tokenSource = new CancellationTokenSource();
        // token取消时从holder中删除
        tokenSource.Token.Register(() =>
        {
            if (Holder.TryGetValue(resourceId, out var tokenSources))
            {
                tokenSources.TryRemove(tokenSource, out _);
            }
        });
        // 请求token取消时同时取消排队token
        requestToken.Register(tokenSource.Cancel);
        var list = Holder.GetOrAdd(resourceId, new ConcurrentDictionary<CancellationTokenSource, CancellationTokenSource>());
        list.TryAdd(tokenSource, tokenSource);

        return tokenSource.Token;
    }

    public void Remove(string resourceId)
    {
        if (Holder.TryGetValue(resourceId, out var cancellationTokenSources))
        {
            foreach (var cancellationTokenSource in cancellationTokenSources.Keys)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            cancellationTokenSources.Clear();
        }
    }
}