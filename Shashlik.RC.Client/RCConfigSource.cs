using Microsoft.Extensions.Configuration;

namespace Shashlik.RC.Client
{
    public class RCConfigSource : IConfigurationSource
    {
        public RCConfigSource(RCOptions options, ApiClient apiClient)
        {
            Options = options;
            ApiClient = apiClient;
        }

        internal RCOptions Options { get; }

        internal ApiClient ApiClient { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new RCConfigProvider(this, ApiClient);
        }
    }
}