using System;
using System.Collections.Generic;
using System.Linq;
using Shashlik.RC.Common;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Cluster
{
    public class ClusterService
    {
        /// <summary>
        /// 获取所有的集群服务器,包括自己
        /// </summary>
        public string[] Servers => _lazyServers.Value;

        private readonly Lazy<string[]> _lazyServers = new(GetServers);

        private static string[] GetServers()
        {
            var servers = Environment.GetEnvironmentVariable(Constants.Envs.Servers);
            if (string.IsNullOrWhiteSpace(servers))
                return Array.Empty<string>();
            return servers.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}