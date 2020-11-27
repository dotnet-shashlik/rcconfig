using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Shashlik.RC.Config
{
    /// <summary>
    /// ini内容解析
    /// </summary>
    class IniParse : IParse
    {
        public string Type => "ini";
        public IDictionary<string, string> Parse(Stream input)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new StreamReader(input))
            {
                var sectionPrefix = string.Empty;

                while (reader.Peek() != -1)
                {
                    var rawLine = reader.ReadLine();
                    var line = rawLine.Trim();

                    // Ignore blank lines
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    // Ignore comments
                    if (line[0] == ';' || line[0] == '#' || line[0] == '/')
                    {
                        continue;
                    }
                    // [Section:header] 
                    if (line[0] == '[' && line[line.Length - 1] == ']')
                    {
                        // remove the brackets
                        sectionPrefix = line.Substring(1, line.Length - 2) + ConfigurationPath.KeyDelimiter;
                        continue;
                    }

                    // key = value OR "value"
                    int separator = line.IndexOf('=');
                    if (separator < 0)
                    {
                        throw new FormatException($"ini format error :{rawLine}");
                    }

                    string key = sectionPrefix + line.Substring(0, separator).Trim();
                    string value = line.Substring(separator + 1).Trim();

                    // Remove quotes
                    if (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"')
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    if (data.ContainsKey(key))
                    {
                        throw new FormatException($"ini format error :{key}");
                    }

                    data[key] = value;
                }
            }

            return data;
        }
    }
}
