using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
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

            var model = $"{Options.Server}/api/Files/{Options.ResourceId}/all"
                .WithHeaders(header)
                .GetJsonAsync<ResponseResult<ResourceModel>>()
                .GetAwaiter().GetResult();

            if (model is null)
                throw new Exception("empty response");
            if (!model.Success)
                throw new Exception("request failed: " + model.Msg);

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
                var res = $"{Options.Server}/api/Files/{Options.ResourceId}/poll"
                    .WithHeaders(header)
                    .WithTimeout(29)
                    .GetJsonAsync<ResponseResult<bool>>()
                    .GetAwaiter().GetResult();

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