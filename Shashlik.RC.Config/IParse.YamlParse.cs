using System.Collections.Generic;
using System.IO;
using NetEscapades.Configuration.Yaml;

namespace Shashlik.RC.Config
{
    /// <summary>
    /// yaml内容解析
    /// </summary>
    internal class YamlParse : IParse
    {
        public string Type => "yaml";

        public IDictionary<string, string> Parse(Stream input)
        {
            return new YamlConfigurationFileParser().Parse(input);
        }
    }
}