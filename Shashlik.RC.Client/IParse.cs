using System.Collections.Generic;
using System.IO;

namespace Shashlik.RC.Client
{
    /// <summary>
    /// 内容解析
    /// </summary>
    internal interface IParse
    {
        string Type { get; }

        IDictionary<string, string> Parse(Stream input);
    }
}
