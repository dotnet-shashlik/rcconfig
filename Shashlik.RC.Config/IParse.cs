using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jinkong.RC.Config
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
