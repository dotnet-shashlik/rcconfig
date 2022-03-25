using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.RC.Client;

internal class InternalService
{
    public static IServiceCollection Services { get; } = new ServiceCollection();

    public static IServiceProvider? ServiceProvider { get; private set; }

    public static void BuildService()
    {
        ServiceProvider = Services.BuildServiceProvider();
    }
}