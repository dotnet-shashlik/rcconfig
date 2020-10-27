using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shashlik.Utils.Helpers;

namespace Jinkong.RC.Config
{
    class RequestHelper
    {
        static SignHelper SignHelper => new SignHelper(RCConfigSource.Instance.SecretKey);

        public static Dictionary<string, object> Get(string config = null)
        {
            var model = new ConfigGetModel { };
            model.config = config;
            model.random = Guid.NewGuid().ToString("n");
            model.env = RCConfigSource.Instance.Env;
            model.appId = RCConfigSource.Instance.AppId;
            var sign = SignHelper.BuildSignModel(model);
            model.sign = sign;

            var result =
                HttpHelper.PostForm(RCConfigSource.Instance.RCServer, model)
                    .GetAwaiter().GetResult();

            var resultObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            if (SignHelper.IsValidSign(resultObj))
                return resultObj;
            else
                throw new Exception("result sign error!");
        }
    }

    class ConfigGetModel
    {
        public string sign { get; set; }

        public string random { get; set; }

        public string appId { get; set; }

        public string env { get; set; }

        public string config { get; set; }
    }
}