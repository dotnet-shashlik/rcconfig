using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shashlik.RC.Client;

internal class JsonConfigurationFileParser
{
    private IDictionary<string, string> Data { get; } = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Stack<string> Context { get; } = new Stack<string>();
    private string _currentPath;
    private JsonTextReader _reader;

    public IDictionary<string, string> ParseStream(Stream input)
    {
        Data.Clear();
        _currentPath = null;
        _reader = new JsonTextReader(new StreamReader(input)) {DateParseHandling = DateParseHandling.None};
        var jObject = JObject.Load(_reader);
        VisitJObject(jObject);
        return Data;
    }

    private void VisitJObject(JObject jObject)
    {
        foreach (var property in jObject.Properties())
        {
            EnterContext(property.Name);
            VisitProperty(property);
            ExitContext();
        }
    }

    private void VisitProperty(JProperty property)
    {
        VisitToken(property.Value);
    }

    private void VisitToken(JToken token)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (token.Type)
        {
            case JTokenType.Object:
                VisitJObject(token.Value<JObject>());
                break;
            case JTokenType.Array:
                VisitArray(token.Value<JArray>());
                break;
            case JTokenType.Integer:
            case JTokenType.Float:
            case JTokenType.String:
            case JTokenType.Boolean:
            case JTokenType.Bytes:
            case JTokenType.Raw:
            case JTokenType.Null:
            case JTokenType.Date:
            case JTokenType.Uri:
            case JTokenType.TimeSpan:
            case JTokenType.Guid:
            case JTokenType.Undefined:
            case JTokenType.None:
                VisitPrimitive(token.Value<JValue>());
                break;
            default:
                throw new FormatException($"Invalid json: {token}");
        }
    }

    private void VisitArray(JArray array)
    {
        for (int index = 0; index < array.Count; index++)
        {
            EnterContext(index.ToString());
            VisitToken(array[index]);
            ExitContext();
        }
    }

    private void VisitPrimitive(JValue data)
    {
        var key = _currentPath;
        if (Data.ContainsKey(key))
            throw new FormatException($"Invalid json: {data}");
        Data[key] = data.ToString(CultureInfo.InvariantCulture);
    }

    private void EnterContext(string context)
    {
        Context.Push(context);
        _currentPath = ConfigurationPath.Combine(Context.Reverse());
    }

    private void ExitContext()
    {
        Context.Pop();
        _currentPath = ConfigurationPath.Combine(Context.Reverse());
    }
}