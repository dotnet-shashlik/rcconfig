using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Common;
using Shashlik.RC.EventBus;

namespace Shashlik.RC.Cluster
{
    [Scoped]
    public class ClusterService
    {
        public ClusterService(ILogger<ClusterService> logger)
        {
            Logger = logger;
        }

        private ILogger<ClusterService> Logger { get; }


        /// <summary>
        /// 是否为单机版
        /// </summary>
        /// <returns></returns>
        public bool IsStandalone()
        {
            return string.IsNullOrWhiteSpace(SystemEnvironmentUtils.Servers);
        }

        /// <summary>
        /// 获取集群所有的服务
        /// </summary>
        /// <returns></returns>
        public string[]? GetServers()
        {
            var servers = SystemEnvironmentUtils.Servers?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            return servers;
        }

        /// <summary>
        /// 事件通知集群
        /// </summary>
        /// <param name="event"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void Notify<T>(T @event) where T : IClusterNotifyEvent
        {
            var servers = GetServers();
            if (!servers.IsNullOrEmpty())
            {
                Parallel.ForEach(servers!, async server =>
                {
                    string requestUrl = $"{server}/internal-server-events/{@event.RequestPath}";

                    bool success = false;
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            var res = await HttpHelper.PostJson(requestUrl, @event, new Dictionary<string, string>
                            {
                                {"TOKEN", SystemEnvironmentUtils.ServerToken}
                            });

                            if (res.Contains("success", StringComparison.OrdinalIgnoreCase))
                            {
                                success = true;
                                break;
                            }

                            Logger.LogWarning($"inner server notify[{requestUrl}] response failed of \"{res}\"");
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e, $"inner server notify[{requestUrl}] occur error on http request");
                            // 根据重试次数延迟相应的时间再重试
                            await Task.Delay(1000 * (i + 1));
                        }
                    }

                    if (!success)
                        Logger.LogError($"inner server[{requestUrl}] notify failed");
                    else
                        Logger.LogDebug($"inner server[{requestUrl}] notify success");
                });
            }
        }
    }
}