using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Shashlik.RC.Utils
{
    /// <summary>
    /// 字符串扩展类
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 是否为null或者空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 字符串比较 忽略大小写
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            if (source == null || target == null)
                return false;
            return source.Equals(target, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 字符串分割为数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static List<T> Split<T>(this string str, params string[] separators) where T : struct
        {
            if (string.IsNullOrWhiteSpace(str))
                return new List<T>();
            return
                str
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(TypeExtensions.ConvertTo<T>).ToList();
        }

        /// <summary>
        /// 字符串分割为数组,跳过不能转换的异常数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static List<T> SplitSkipError<T>(this string str, params string[] separators) where T : struct
        {
            if (string.IsNullOrWhiteSpace(str))
                return new List<T>();
            var query =
                str
                .Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<T> result = new List<T>();
            foreach (var item in query)
            {
                try
                {
                    result.Add(item.ConvertTo<T>());
                }
                catch { }
            }
            return result;
        }

        public static bool Contains(this string source, string value, StringComparison stringComparison)
        {
            if (source == null || value == null) { return false; }
            if (value == "") { return true; }
            return (source.IndexOf(value, stringComparison) >= 0);
        }

        /// <summary>
        /// 字符串截取,null时,返回"",不足长度时,返回字符串本身
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubStringIfTooLong(this string str, int length, string suffix = "...")
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";

            var span = str.AsSpan().Trim();
            if (span.Length <= length)
                return span.ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append(span.Slice(0, length).ToString());
            sb.Append(suffix);
            return sb.ToString();
        }

        /// <summary>
        /// 字符串替换,忽略大小写,使用的是正则,注意正则中的特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="replaceString"></param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string str, string pattern, string replaceString)
        {
            return Regex.Replace(str, pattern, replaceString, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 正则是否匹配
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regexPattern">正则表达式</param>
        /// <returns></returns>
        public static bool IsMatch(this string value, string regexPattern)
        {
            if (value == null)
                return false;
            return Regex.IsMatch(value, regexPattern);
        }

        /// <summary>
        /// 正则是否匹配
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUrl(this string value)
        {
            if (value == null)
                return false;
            return Regex.IsMatch(value, @"^((https|http)?:\/\/)[^\s]+");
        }


        /// <summary>
        /// 空值转换为null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EmptyToNull(this string value)
        {
            return value.IsNullOrWhiteSpace() ? null : value;
        }

        /// <summary>
        /// 字符串格式化,string.Formart
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static string Format(this string value, params object[] ps)
        {
            return string.Format(value, ps);
        }

        /// <summary>
        /// razor模板格式化,参数格式:@{arg},参数名区分大小写,支持数据类型本身的格式化format,如@{Money|f2},Money为double类型,|f2表示格式化为2位小数显示
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="value">值</param>
        /// <param name="model">格式化模型</param>
        /// <returns></returns>
        public static string RazorFormat<TModel>(this string value, TModel model)
        {
            if (value.IsNullOrWhiteSpace())
                return value;
            if (model == null)
                return value;

            // 特殊字符
            char[] specialChar = { '$', '(', ')', '*', '+', '.', '[', ']', '{', '}', '?', '\\', '^', '|' };
            // 变量前缀
            char prefix = '@';
            var reg = new Regex(prefix + @"\{[^\{\}]{1,}\}");

            var matches = reg.Matches(value);
            if (matches.Count == 0)
                return value;

            //var type = typeof(TModel);
            //var properties = type.GetProperties();

            foreach (Match item in matches)
            {
                if (!item.Success)
                    continue;
                string exp = item.Value.Trim().TrimStart(prefix).TrimStart('{').TrimEnd('}');

                string proName = null;
                string format = null;

                if (exp.Contains('|'))
                {
                    var arr = exp.Split('|');
                    if (arr.Length != 2)
                        throw new System.Exception($"RazorFormat发生错误:表达式[{exp}]错误");

                    proName = arr[0].Trim();
                    format = arr[1].Trim();
                }
                else
                    proName = exp;

                var proValue = ProValue(model, proName);
                if (!proValue.existsPro)
                    continue;
                var v = proValue.value;

                if (format.IsNullOrWhiteSpace())
                    value = value.Replace(item.Value, v?.ToString() ?? "");
                else
                {
                    var method = v.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                           .Where(r =>
                           {

                               var ps = r.GetParameters();
                               return r.Name == "ToString" && ps.Length == 1 && ps[0].Name == "format";
                           })
                           .FirstOrDefault();
                    if (method == null)
                        throw new System.Exception($"RazorFormat发生错误:类型[{v.GetType()}]没有格式化方法:[{exp}]");

                    var s = method.Invoke(v, new object[] { format });

                    value = value.Replace(item.Value, s?.ToString() ?? "");

                }
            }

            return value;
        }

        /// <summary>
        /// razor模板格式化,参数格式:@{arg},参数名区分大小写,支持数据类型本身的格式化format,如@{Money|f2},Money为double类型,|f2表示格式化为2位小数显示
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="value">值</param>
        /// <param name="model">格式化模型</param>
        /// <returns></returns>
        public static string RazorFormatForDictionary(this string value, IDictionary<string, object> model)
        {
            if (value.IsNullOrWhiteSpace())
                return value;
            if (model == null)
                return value;

            // 特殊字符
            char[] specialChar = { '$', '(', ')', '*', '+', '.', '[', ']', '{', '}', '?', '\\', '^', '|' };
            // 变量前缀
            char prefix = '@';
            var reg = new Regex(prefix + @"\{[^\{\}]{1,}\}");

            var matches = reg.Matches(value);
            if (matches.Count == 0)
                return value;

            //var type = typeof(TModel);
            //var properties = type.GetProperties();

            foreach (Match item in matches)
            {
                if (!item.Success)
                    continue;
                string exp = item.Value.Trim().TrimStart(prefix).TrimStart('{').TrimEnd('}');

                string proName = null;
                string format = null;

                if (exp.Contains('|'))
                {
                    var arr = exp.Split('|');
                    if (arr.Length != 2)
                        throw new System.Exception($"RazorFormat发生错误:表达式[{exp}]错误");

                    proName = arr[0].Trim();
                    format = arr[1].Trim();
                }
                else
                    proName = exp;

                if (!model.ContainsKey(proName))
                    continue;

                var v = model.GetOrDefault(proName);
                if (format.IsNullOrWhiteSpace())
                    value = value.Replace(item.Value, v?.ToString() ?? "");
                else
                {
                    var method = v.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                           .Where(r =>
                           {

                               var ps = r.GetParameters();
                               return r.Name == "ToString" && ps.Length == 1 && ps[0].Name == "format";
                           })
                           .FirstOrDefault();
                    if (method == null)
                        throw new System.Exception($"RazorFormat发生错误:类型[{v.GetType()}]没有格式化方法:[{exp}]");

                    var s = method.Invoke(v, new object[] { format });

                    value = value.Replace(item.Value, s?.ToString() ?? "");
                }
            }

            return value;
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="proName">属性名</param>
        /// <returns></returns>
        static (object value, bool existsPro) ProValue(object obj, string proName)
        {
            if (!proName.Contains('.'))
            {
                var ps = obj.GetType().GetProperties();
                var pro = ps.FirstOrDefault(r => r.Name == proName);
                if (pro == null)
                    return (null, false);
                return (pro.GetValue(obj), true);
            }

            else
            {
                var proNameArr = proName.Split('.');

                var ps = obj.GetType().GetProperties();
                var pro = ps.FirstOrDefault(r => r.Name == proNameArr[0]);
                if (pro == null)
                    return (null, false);
                var proNameList = proNameArr.ToList();
                proNameList.RemoveAt(0);
                var resStr = proNameList.Join(".");
                return ProValue(pro.GetValue(obj), resStr);
            }
        }

        /// <summary>
        /// 字符串脱敏
        /// </summary>
        /// <param name="value"></param>
        /// <param name="beginLength">前面保留几位</param>
        /// <param name="endLength">后面保留几位</param>
        /// <returns></returns>
        public static string ConfidentialData(this string value, int beginLength, int endLength)
        {
            if (value.IsNullOrWhiteSpace())
                return "";

            var span = value.AsSpan();
            StringBuilder sb = new StringBuilder();
            if (value.Length >= beginLength)
                sb.Append(span.Slice(0, beginLength).ToString());
            else
                return value;

            sb.Append("****");

            if (value.Length >= endLength)
                sb.Append(span.Slice(value.Length - endLength).ToString());

            return sb.ToString();
        }

        public static bool StartsWithIgnoreCase(this string value, string starts)
        {
            return value.AsSpan().StartsWith(starts.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(this string value, string ends)
        {
            return value.AsSpan().EndsWith(ends.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        public static string HtmlEncode(this string value)
        {
            if (value == null)
                return null;
            return HttpUtility.HtmlEncode(value);
        }

        public static string HtmlDecode(this string value)
        {
            if (value == null)
                return null;
            return HttpUtility.HtmlDecode(value);
        }

        public static string UrlEncode(this string value)
        {
            if (value == null)
                return null;
            return HttpUtility.UrlEncode(value);
        }

        public static string UrlDecode(this string value)
        {
            if (value == null)
                return null;
            return HttpUtility.UrlDecode(value);
        }

        /// <summary>
        /// url参数合并
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string UrlArgsCombine(this string url, IEnumerable<KeyValuePair<string, object>> values)
        {
            if (url.IsNullOrWhiteSpace() || values.IsNullOrEmpty())
                return url;

            StringBuilder sb = new StringBuilder();
            sb.Append(url);
            var span = url.AsSpan().Trim();
            if (!span.Contains(new[] { '?' }, StringComparison.OrdinalIgnoreCase))
                sb.Append('?');
            else if (!span.EndsWith(new[] { '&' }, StringComparison.OrdinalIgnoreCase))
                sb.Append('&');

            var count = values.Count();
            for (int i = 0; i < count; i++)
            {
                var item = values.ElementAt(i);
                if (item.Value == null)
                    continue;

                sb.Append(item.Key);
                sb.Append('=');
                sb.Append(item.Value.ToString().UrlEncode());
                sb.Append('&');
            }

            return sb.ToString().TrimEnd('&');
        }

        /// <summary>
        /// MD5 32位
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md532(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("message", nameof(input));
            }

            return SecurityHelper.Md532(input);
        }

        /// <summary>
        /// 清除字符串中的html标签
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string input)
        {
            var regex = new Regex(@"<.*?>");
            return regex.Replace(input, "");
        }

        /// <summary>
        /// 按文本字符截取字符串，通常用于包含emoji的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubStringByTextElements(this string input, int start, int length)
        {
            var stringInfo = new StringInfo(input);
            if (start == 0 && stringInfo.LengthInTextElements <= length)
            {
                return input;
            }
            return stringInfo.SubstringByTextElements(start, length);
        }

        #region Base64位加密解密

        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        public static string Base64Encode(this string content, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static string Base64Decode(this string content, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            byte[] bytes = Convert.FromBase64String(content);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static byte[] Base64DecodeToBytes(this string content, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return Convert.FromBase64String(content);
        }

        #endregion
    }
}