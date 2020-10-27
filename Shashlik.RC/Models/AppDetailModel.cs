using System.Collections.Generic;

namespace Shashlik.RC.Models
{
    public class AppDetailModel
    {
        public string AppId { get; set; }

        public string SecretKey { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public bool Enabled { get; set; }

        public List<EnvModel> Envs { get; set; }
    }
}
