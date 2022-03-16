using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Events;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Server.EventBus;
using Shashlik.RC.Server.Common;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Cluster
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
        /// 事件通知集群
        /// </summary>
        /// <param name="event"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void InnerNotify(object @event)
        {
            var servers = SystemEnvironmentUtils.Servers;
            if (!servers.IsNullOrEmpty())
            {
                Parallel.ForEach(servers!, async server =>
                {
                    string requestUrl = $"{server}/InnerServer/event";

                    bool success = false;
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            var res = await HttpHelper.PostJson(requestUrl, @event, new Dictionary<string, string>
                            {
                                {Constants.HeaderKeys.ServerToken, SystemEnvironmentUtils.ServerToken},
                                {Constants.HeaderKeys.EventType, @event.GetType().FullName!}
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