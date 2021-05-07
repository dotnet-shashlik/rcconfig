#nullable disable
using System.Collections.Generic;

namespace Shashlik.RC.Common
{
    public class PageModel<T>
    {
        public int Total { get; set; }

        public List<T> Rows { get; set; }
    }
}