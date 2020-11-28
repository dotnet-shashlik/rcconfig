using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Config
{
    public class RCConfigProvider : ConfigurationProvider
    {
        public RCConfigProvider(RCConfigSource source)
        {
            Source = source;

            // 配置发送变更时,更新配置
            ConfigOnChange.OnChange += (config) => Load();

            if (source.Polling.HasValue)
                Polling();
        }

        private RCConfigSource Source { get; }

        /// <summary>
        /// 轮询更新配置
        /// </summary>
        private void Polling()
        {
            if (Source.Polling.HasValue)
                TimerHelper.SetInterval(() =>
                {
                    try
                    {
                        Load();
                    }
                    catch
                    {
                        // ignored
                    }
                }, Source.Polling.Value);
        }

        public override void Load()
        {
            var result = RequestHelper.Get();
            var configs = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result["data"]!.ToString()!);
            // ReSharper disable once InconsistentNaming
            IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in configs)
            {
                string name = item["name"], type = item["type"], content = item["content"];
                if (name.IsNullOrWhiteSpace() || type.IsNullOrWhiteSpace())
                    throw new Exception("name or type is empty!");
                if (content.IsNullOrWhiteSpace())
                    continue;

                using var serviceProvider = InternalService.Services.BuildServiceProvider();
                var parseList = serviceProvider.GetServices<IParse>();
                var parse = parseList.SingleOrDefault(r => r.Type.EqualsIgnoreCase(type));
                if (parse == null)
                    throw new Exception($"invalid configuration type: {type}");
                using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
                var data = parse.Parse(ms);
                foreach (var keyValue in data)
                    _data.Add(keyValue.Key, keyValue.Value);
            }

            Data = _data;
        }
    }
}