using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Utils
{
    /// <summary>
    /// 参数签名帮助类
    /// </summary>
    public class SignHelper
    {
        public SignHelper(string secretKey)
        {
            SecretKey = secretKey;
        }

        private string SecretKey { get; }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public string BuildSign(IDictionary<string, string> ps)
        {
            var key = SecretKey;
            var list = ps.Where(r => r.Value != null && !r.Value.IsNullOrWhiteSpace());
            list = list.Select(r => new KeyValuePair<string, string>(r.Key.Trim().ToLower(), r.Value)).OrderBy(r => r.Key);
            var str = list.Select(r => $"{r.Key}={r.Value}").Join("&");
            str += $"&appkey={key}";
            return HashHelper.MD5(str).ToUpperInvariant();
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public string BuildSignJObject(JObject jObject)
        {
            IDictionary<string, string> ps = new Dictionary<string, string>();
            foreach (var item in jObject)
                ps.Add(item.Key, item.Value!.Value<string>());

            return BuildSign(ps);
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string BuildSignModel<TModel>(TModel model)
            where TModel : class
        {
            return BuildSign(model
                .MapToDictionary()
                .ToDictionary(r => r.Key, r => r.Value?.ToString())
            );
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public bool IsValidSign(IEnumerable<KeyValuePair<string, object>> ps)
        {
            var key = SecretKey;
            var keyValuePairs = ps.ToList();
            var psSign = keyValuePairs.FirstOrDefault(r => r.Key == "sign").Value;
            if (psSign == null || psSign.ToString().IsNullOrWhiteSpace())
                return false;

            ps = keyValuePairs.Where(r => r.Key != "sign" && r.Value != null && !r.Value.ToString().IsNullOrWhiteSpace()).ToList();
            if (!ps.Any())
                return false;

            var list = ps.Select(r => new KeyValuePair<string, string>(r.Key.Trim().ToLower(), r.Value?.ToString())).OrderBy(r => r.Key).ToList();
            var str = list.Select(r => $"{r.Key.Trim().ToLower()}={r.Value}").Join("&");
            str += $"&appkey={key}";
            return HashHelper.MD5(str).ToUpperInvariant() == psSign.ToString();
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public bool IsValidSignJObject(JObject jObject)
        {
            List<KeyValuePair<string, object>> ps = new List<KeyValuePair<string, object>>();
            foreach (var item in jObject)
                ps.Add(new KeyValuePair<string, object>(item.Key.Trim().ToLower(), item.Value!.Value<object>()));
            return IsValidSign(ps);
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsValidSignModel<TModel>(TModel model)
            where TModel : class
        {
            return IsValidSign(model.MapToDictionary());
        }
    }
}