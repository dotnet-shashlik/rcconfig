#nullable disable
using System.Collections.Generic;

namespace Shashlik.RC.Server.Common
{
    public class PageModel<T>
    {
        public PageModel()
        {
        }

        public PageModel(long total, List<T> rows)
        {
            Total = total;
            Rows = rows;
        }

        public long Total { get; set; }

        public List<T> Rows { get; set; }
    }
}