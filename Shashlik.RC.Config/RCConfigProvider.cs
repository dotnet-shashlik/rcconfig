using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Newtonsoft.Json;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Jinkong.RC.Config
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

        RCConfigSource Source { get; }

        /// <summary>
        /// 轮询更新配置
        /// </summary>
        private void Polling()
        {
            TimerHelper.SetTimeout(() =>
            {
                try
                {
                    Load();
                    Polling();
                }
                catch { }

            }, Source.Polling.Value);
        }

        public override void Load()
        {
            var result = RequestHelper.Get();
            var configs = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result["data"].ToString());
            IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in configs)
            {
                string name = item["name"], type = item["type"], content = item["content"];
                if (name.IsNullOrWhiteSpace() || type.IsNullOrWhiteSpace())
                    throw new Exception("name or type is empty!");
                if (content.IsNullOrWhiteSpace())
                    continue;

                // 如果是纯文本,name为key,content为value
                if (type.EqualsIgnoreCase("text"))
                {
                    _data.Add(name, content);
                    continue;
                }

                using (var serviceProvider = InternalService.Services.BuildServiceProvider())
                {
                    var parseList = serviceProvider.GetServices<IParse>();
                    var parse = parseList.SingleOrDefault(r => r.Type.EqualsIgnoreCase(type));
                    if (parse == null)
                        throw new Exception($"invalid configuration type:{type}");
                    using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
                    {
                        var data = parse.Parse(ms);
                        foreach (var keyValue in data)
                            _data.Add(keyValue.Key, keyValue.Value);
                    }
                }
            }

            Data = _data;
        }
    }
}
