using Microsoft.Extensions.Configuration;
using System;
using Shashlik.Utils.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Websocket.Client;
using System.Threading;
using System.Reactive.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shashlik.Utils.Helpers;

// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable InconsistentNaming

namespace Jinkong.RC.Config
{
    public static class Extensions
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
            builder.ConfigureServices((context, services) =>
            {
                // 这个配置给全局用
                services.AddSingleton<IRCConfigManager, RCConfigManager>();
            });

            // 内部服务
            InternalService.Services.AddSingleton<IRCConfigManager, RCConfigManager>();
            InternalService.Services.AddSingleton<IParse, JsonParse>();
            InternalService.Services.AddSingleton<IParse, XmlParse>();
            InternalService.Services.AddSingleton<IParse, IniParse>();
            InternalService.Services.AddSingleton<IParse, YamlParse>();

            builder.ConfigureAppConfiguration((host, config) =>
            {
                var configurationRoot = config.Build();
                var options = configurationRoot.GetSection("RCConfig").Get<RCOptions>();

                if (options.Polling.HasValue && options.Polling < 0)
                    throw new InvalidOperationException("invalid rc options value of: RC.Polling. ");

                var source = new RCConfigSource(options.Server, options.AppId, options.AppKey,
                    options.Polling.HasValue ? TimeSpan.FromSeconds(options.Polling.Value) : (TimeSpan?)null)
                {
                    Env = host.HostingEnvironment.EnvironmentName
                };

                config.AddRCConfigProvider(source);
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
            });

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