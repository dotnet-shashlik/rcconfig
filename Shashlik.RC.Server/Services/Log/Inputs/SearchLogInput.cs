using System.ComponentModel.DataAnnotations;
using Shashlik.RC.Server.Common;

namespace Shashlik.RC.Server.Services.Log.Inputs
{
    public class SearchLogInput : PageInput
    {
        public int? FileId { get; set; }

        [StringLength(255)] public string? FileName { get; set; }
    }
}