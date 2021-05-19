#nullable disable
using System.Collections.Generic;

namespace Shashlik.RC.Common
{
    public class PageModel<T>
    {
        public PageModel()
        {
        }

        public PageModel(int total, List<T> rows)
        {
            Total = total;
            Rows = rows;
        }

        public int Total { get; set; }

        public List<T> Rows { get; set; }
    }
}