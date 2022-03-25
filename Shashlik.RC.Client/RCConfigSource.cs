using Microsoft.Extensions.Configuration;

namespace Shashlik.RC.Client;

public class RCConfigSource : IConfigurationSource
{
    public RCConfigSource(RCClientOptions options, ApiClient apiClient)
    {
        Options = options;
        ApiClient = apiClient;
    }

    internal RCClientOptions Options { get; }

    internal ApiClient ApiClient { get; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new RCConfigProvider(this, ApiClient);
    }
}