using System.Collections.Generic;
using Shashlik.RC.Utils;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Models
{
    public class EnvModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        public List<string> IpWhites { get; set; }

        public string IpWhitesStr
        {
            get
            {
                if (IpWhites.IsNullOrEmpty())
                    return null;
                return IpWhites.Join("\n");
            }
        }

        public List<ConfigSimpleModel> Configs { get; set; }
    }
}
