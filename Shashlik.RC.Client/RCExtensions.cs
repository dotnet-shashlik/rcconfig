using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shashlik.RC.Client;

public static class RCExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddRCConfigProvider(this IConfigurationBuilder builder,
        RCConfigSource source)
    {
        builder.Add(source);
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder UseRCConfiguration(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((host, config) =>
        {
            var configurationRoot = config.Build();
            var options = configurationRoot.GetSection("RCConfig").Get<RCClientOptions>();
            // 内部服务
            InternalService.Services.AddSingleton<IParse, JsonParse>();
            InternalService.Services.AddSingleton<IParse, YamlParse>();
            InternalService.Services.AddSingleton(options);
            InternalService.Services.AddSingleton<ApiClient>();
            InternalService.BuildService();

            var source = new RCConfigSource(options, InternalService.ServiceProvider!.GetRequiredService<ApiClient>());
            config.AddRCConfigProvider(source);
        });

        builder.ConfigureServices((context, services) =>
        {
            services.Configure<RCClientOptions>(context.Configuration.GetSection("RCConfig"));
        });

        return builder;
    }
}