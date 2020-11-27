using System;
using Microsoft.Extensions.Configuration;

namespace Shashlik.RC.Config
{
    public class RCConfigSource : IConfigurationSource
    {
        public RCConfigSource(string rCServer, string appId, string secretKey, TimeSpan? polling = null)
        {
            RCServer = rCServer;
            AppId = appId;
            SecretKey = secretKey;
            if (polling.HasValue && polling.Value.TotalMilliseconds < 0)
                throw new ArgumentException("轮询时间必须大于0", nameof(polling));

            Polling = polling;
        }

        /// <summary>
        /// RC服务器地址
        /// </summary>
        internal string RCServer { get; set; }

        /// <summary>
        /// 应用id
        /// </summary>
        internal string AppId { get; set; }

        /// <summary>
        /// SecretKey
        /// </summary>
        internal string SecretKey { get; set; }

        /// <summary>
        /// 环境变量
        /// </summary>
        internal string Env { get; set; }

        /// <summary>
        /// 轮询加载配置,配置不开启轮询
        /// </summary>
        internal TimeSpan? Polling { get; set; }

        internal static RCConfigSource Instance { get; private set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            Instance = this;
            return new RCConfigProvider(this);
        }
    }
}
