﻿using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Utils;
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
                        context.Request.Query.TryGetValue("appId", out var appId);
                        context.Request.Query.TryGetValue("env", out var env);
                        context.Request.Query.TryGetValue("sign", out var sign);
                        context.Request.Query.TryGetValue("timestamp", out var timestamp);
                        if (appId.IsNullOrEmpty()
                            || env.IsNullOrEmpty()
                            || sign.IsNullOrEmpty()
                            || timestamp.ToString().IsNullOrEmpty())
                        {
                            context.Response.StatusCode = 400;
                            return;
                        }

                        var dbContext = context.RequestServices.GetService<RCDbContext>();
                        var appIdStr = appId.ToString();
                        var envStr = env.ToString();
                        var signStr = sign.ToString();
                        var appEntity = dbContext.Set<Apps>().Include(r => r.Envs)
                            .FirstOrDefault(r => r.Id == appIdStr);
                        if (appEntity is null)
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }

                        var envEntity = appEntity.Envs.FirstOrDefault(r => r.Name == envStr);
                        if (!appEntity.Enabled || envEntity is null)
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }

                        var key = $"appid={appIdStr}&env={envStr}&timestamp={timestamp}";
                        if (HashHelper.HMACSHA256(key, envEntity.Key) != signStr)
                        {
                            context.Response.StatusCode = 403;
                            return;
                        }

                        //接受新的ws请求，并生成新的guid
                        var socket = await context.WebSockets.AcceptWebSocketAsync();
                        //把所有的在线socket统一存放
                        await context.RequestServices.GetRequiredService<WebSocketContext>()
                            .AddSocket(appIdStr, envStr, socket);
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