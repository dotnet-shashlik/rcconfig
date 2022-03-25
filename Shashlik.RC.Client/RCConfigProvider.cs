using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Client;

public class RCConfigProvider : ConfigurationProvider
{
    public RCConfigProvider(RCConfigSource source, ApiClient apiClient)
    {
        Source = source;
        ApiClient = apiClient;
        Task.Run(Poll);
    }

    private RCConfigSource Source { get; }
    private ApiClient ApiClient { get; }


    private void Poll()
    {
        while (true)
        {
            if (ApiClient.Poll())
            {
                Load();
            }
        }
    }

    public override void Load()
    {
        var files = ApiClient.GetFiles();
        IDictionary<string, string> d = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in files.Files)
        {
            string name = item.Name, type = item.Type, content = item.Content;
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
                d.Add(keyValue.Key, keyValue.Value);
        }

        Data = d;
        OnReload();
    }
}