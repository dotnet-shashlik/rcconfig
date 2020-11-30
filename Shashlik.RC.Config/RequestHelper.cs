using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shashlik.Utils.Helpers;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Config
{
    internal class RequestHelper
    {
        static SignHelper SignHelper => new SignHelper(RCConfigSource.Instance.Options.AppKey);

        public static Dictionary<string, object> Get(string config = null)
        {
            var model = new ConfigGetModel
            {
                config = config, random = Guid.NewGuid().ToString("n"), env = RCConfigSource.Instance.Env,
                appId = RCConfigSource.Instance.Options.AppId
            };
            var sign = SignHelper.BuildSignModel(model);
            model.sign = sign;

            var result =
                HttpHelper.PostForm(RCConfigSource.Instance.Options.ApiUrl, model)
                    .GetAwaiter().GetResult();

            var resultObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            if (SignHelper.IsValidSign(resultObj))
                return resultObj;
            else
                throw new Exception("result sign error!");
        }
    }

    internal class ConfigGetModel
    {
        public string sign { get; set; }

        public string random { get; set; }

        public string appId { get; set; }

        public string env { get; set; }

        public string config { get; set; }
    }
}