using System.Collections.Generic;
using System.IO;

namespace Shashlik.RC.Client;

/// <summary>
/// json内容解析
/// </summary>
internal class JsonParse : IParse
{
    public string Type => "json";
    public IDictionary<string, string> Parse(Stream input) => new JsonConfigurationFileParser().ParseStream(input);
}