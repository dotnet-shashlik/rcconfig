using System;

namespace Shashlik.RC.Models
{
    public class ModifyRecordModel
    {
        public int Id { get; set; }

        public int ConfigId { get; set; }

        public string ConfigName { get; set; }

        public string ConfigDesc { get; set; }

        public DateTime ModifyTime { get; set; }

        public string Desc { get; set; }

        public string Type { get; set; }

        public string BeforeContent { get; set; }

        public string AfterContent { get; set; }
    }
}
