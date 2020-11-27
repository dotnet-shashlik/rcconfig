using System.Collections.Generic;
using System.IO;

namespace Shashlik.RC.Config
{
    /// <summary>
    /// 内容解析
    /// </summary>
    interface IParse
    {
        string Type { get; }

        IDictionary<string, string> Parse(Stream input);
    }
}
