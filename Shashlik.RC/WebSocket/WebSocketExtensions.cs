using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Services.Environment;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.WebSocket
{
    public static class WebSocketExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        public static void UseWebSocketPush(this IApplicationBuilder app, string path = "/ws/subscribe")
        {
            app.UseWebSockets();

            app.Map(path, builder =>
            {
                builder.Run(async context =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("WebsocketServer");

                        context.Request.Query.TryGetValue("secretId", out var secretId);
                        context.Request.Query.TryGetValue("sign", out var sign);
                        context.Request.Query.TryGetValue("timestamp", out var timestamp);
                        if (secretId.IsNullOrEmpty()
                            || sign.IsNullOrEmpty()
                            || timestamp.ToString().IsNullOrEmpty())
                        {
                            context.Response.StatusCode = 400;
                            return;
                        }

                        if (!timestamp.ToString().TryParse<long>(out var timestampLong)
                            || Math.Abs(DateTime.Now.GetLongDate() - timestampLong) > 60 * 2
                        )
                        {
                            logger.LogDebug($"invalid timestamp: {timestamp}");
                            context.Response.StatusCode = 400;
                            return;
                        }

                        var environmentService = context.RequestServices.GetRequiredService<EnvironmentService>();
                        var secretIdStr = secretId.ToString();
                        var signStr = sign.ToString();
                        var environment = await environmentService.GetBySecretId(secretIdStr);
                        if (environment is null)
                        {
                            logger.LogDebug($"invalid secretId: {secretIdStr}");
                            context.Response.StatusCode = 400;
                            return;
                        }

                        var key = $"secretId={secretIdStr}&timestamp={timestamp}";
                        if (HashHelper.HMACSHA256(key, environment.Secrets.First().SecretKey) != signStr)
                        {
                            logger.LogDebug($"invalid signature");
                            context.Response.StatusCode = 400;
                            return;
                        }

                        var socket = await context.WebSockets.AcceptWebSocketAsync();
                        //把所有的在线socket统一存放
                        await context.RequestServices.GetRequiredService<WebSocketContext>()
                            .AddSocket(environment.ResourceId, socket);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                });
            });
        }
    }
}