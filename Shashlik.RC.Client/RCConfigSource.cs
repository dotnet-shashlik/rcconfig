using System;
using Microsoft.Extensions.Configuration;

namespace Shashlik.RC.Config
{
    public class RCConfigSource : IConfigurationSource
    {
        public RCConfigSource(RCOptions options, string env, TimeSpan? polling = null)
        {
            Options = options;
            Env = env;
            Polling = polling;
        }

        internal RCOptions Options { get; }

        /// <summary>
        /// 环境变量
        /// </summary>
        internal string Env { get; }

        /// <summary>
        /// 轮询加载配置,配置不开启轮询
        /// </summary>
        internal TimeSpan? Polling { get; }

        internal static RCConfigSource Instance { get; private set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            Instance = this;
            return new RCConfigProvider(this);
        }
    }
}