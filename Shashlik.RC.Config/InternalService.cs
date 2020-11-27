using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.RC.Config
{
    internal class InternalService
    {
        public static IServiceCollection Services { get; } = new ServiceCollection();
    }
}