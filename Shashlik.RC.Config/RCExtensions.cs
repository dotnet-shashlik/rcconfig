using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;
using Websocket.Client;

// ReSharper disable UseObjectOrCollectionInitializer

// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Config
{
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
            // 内部服务
            InternalService.Services.AddSingleton<IRCConfigManager, RCConfigManager>();
            InternalService.Services.AddSingleton<IParse, JsonParse>();
            InternalService.Services.AddSingleton<IParse, YamlParse>();

            builder.ConfigureAppConfiguration((host, config) =>
            {
                var configurationRoot = config.Build();
                var options = configurationRoot.GetSection("RCConfig").Get<RCOptions>();

                if (options.Polling < 0)
                    throw new InvalidOperationException("invalid rc options value of: RC.Polling. ");

                var source = new RCConfigSource(options, host.HostingEnvironment.EnvironmentName,
                    options.Polling > 0 ? TimeSpan.FromSeconds(options.Polling) : (TimeSpan?) null);

                config.AddRCConfigProvider(source);
            });

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IRCConfigManager, RCConfigManager>();
                services.Configure<RCOptions>(context.Configuration.GetSection("RCConfig"));
            });

            return builder;
        }

        private static readonly object GATE1 = new object();

        /// <summary>
        /// 使用websocket连接rc实时更新
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="websocketAddress"></param>
        public static async void UseRCRealTimeUpdate(this IApplicationBuilder app, string appId, string secret,
            string websocketAddress)
        {
            var t = DateTime.Now.GetLongDate();
            var key =
                $"appid={appId}&env={app.ApplicationServices.GetService<IHostEnvironment>().EnvironmentName}&timestamp={t}";
            var sign = HashHelper.HMACSHA256(key, secret);
            key += $"&sign={sign.UrlEncode()}";
            var url = new Uri(websocketAddress + "?" + key);
            var logger = app.ApplicationServices.GetService<ILoggerFactory>().CreateLogger("rc.websocket");

            var client = new WebsocketClient(url);
            // 服务端是2分钟
            client.ReconnectTimeout = TimeSpan.FromSeconds(100);
            client.DisconnectionHappened.Subscribe(info =>
            {
                if (info.Exception != null)
                    logger.LogWarning(
                        $"Disconnection happened, type: {info.Type}, ex:{info.Exception}, CancelReconnection:{info.CancelReconnection}");
            });


            var breakSource = new CancellationTokenSource();
            _ = Task.Run(() =>
            {
                while (true)
                {
                    if (breakSource.Token.IsCancellationRequested)
                        break;
                    Thread.Sleep(30 * 1000);
                    client.Send("heartbeat");
                }
            }, breakSource.Token);

            client.MessageReceived
                .Synchronize(GATE1)
                .Subscribe(msg =>
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<RefreshModel>(msg.Text);
                        if (data.Command == "refresh")
                        {
                            logger.LogWarning($"Rc configuration refreshed, content: {msg.Text}");
                            ConfigOnChange.OnChange?.Invoke(data.Data.Config);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"refresh configuration error.");
                    }
                });

            AppDomain.CurrentDomain.ProcessExit += (a, b) =>
            {
                breakSource.Cancel();
                try
                {
                    breakSource.Dispose();
                    client.Dispose();
                }
                catch
                {
                    // ignored
                }
            };

            await client.Start();
        }

        public static void UseRCRealTimeUpdate(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<RCOptions>>().Value;
            UseRCRealTimeUpdate(app, options.AppId, options.AppKey, options.Websocket);
        }

        public class RefreshModel
        {
            public string Command { get; set; }

            public _Data Data { get; set; }

            public class _Data
            {
                public string Config { get; set; }
            }
        }
    }
}