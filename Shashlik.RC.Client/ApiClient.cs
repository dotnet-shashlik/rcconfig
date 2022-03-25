using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Shashlik.RC.Client.Models;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;


namespace Shashlik.RC.Client
{
    public class ApiClient
    {
        public ApiClient(RCClientOptions rcOptions)
        {
            Options = rcOptions;
        }

        private RCClientOptions Options { get; }
        private long _version;

        private string BuildSign(string nonce, long timestamp, IDictionary<string, string?>? query, string? body)
        {
            var signData = new Dictionary<string, string?>();
            if (query is not null)
                foreach (var keyValuePair in query)
                    signData[keyValuePair.Key] = keyValuePair.Value;
            signData["Body"] = body;
            signData["SecretId"] = Options.SecretId;
            signData["Timestamp"] = timestamp.ToString();
            signData["Nonce"] = nonce;

            var signStr = signData.OrderBy(r => r.Key)
                .Select(r => $"{r.Key}={r.Value}")
                .Join("&");
            return HashHelper.HMACSHA256(signStr, Options.SecretKey);
        }

        public ResourceModel GetFiles()
        {
            var nonce = Guid.NewGuid().ToString("n");
            var timestamp = DateTime.Now.GetLongDate();
            var sign = BuildSign(nonce, timestamp, null, null);
            var header = new Dictionary<string, string?>
            {
                { "SecretId", Options.SecretId },
                { "Timestamp", timestamp.ToString() },
                { "Sign", sign },
                { "Nonce", nonce },
            };
            var response = HttpHelper.GetString($"{Options.Server}/api/Files/{Options.ResourceId}/all",
                null, header).GetAwaiter().GetResult();

            var model = JsonConvert.DeserializeObject<ResponseResult<ResourceModel>>(response!);
            if (model is null)
                throw new Exception("empty response");
            if (!model.Success)
                throw new Exception("request failed: " + response);

            _version = model.Data!.Version;
            return model.Data;
        }

        public bool Poll()
        {
            var query = new Dictionary<string, string?>
            {
                { "version", _version.ToString() }
            };
            var nonce = Guid.NewGuid().ToString("n");
            var timestamp = DateTime.Now.GetLongDate();
            var sign = BuildSign(nonce, timestamp, query, null);
            var header = new Dictionary<string, string?>
            {
                { "SecretId", Options.SecretId },
                { "Timestamp", timestamp.ToString() },
                { "Sign", sign },
                { "Nonce", nonce },
            };
            try
            {
                var response = HttpHelper.GetString($"{Options.Server}/api/Files/{Options.ResourceId}/poll",
                    query, header, null, timeout: 29).GetAwaiter().GetResult();

                var res = JsonConvert.DeserializeObject<ResponseResult<bool>>(response!);
                if (res is null)
                    return false;
                if (res.Success)
                    return res.Data;
                return false;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }
    }
}