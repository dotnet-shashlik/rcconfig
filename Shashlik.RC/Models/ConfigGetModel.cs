using System.ComponentModel.DataAnnotations;

namespace Shashlik.RC.Models
{
    public class ConfigGetModel
    {
        [Required, MaxLength(32), MinLength(32)]
        public string sign { get; set; }

        [Required, MaxLength(32), MinLength(32)]
        public string random { get; set; }

        [Required, MaxLength(16), MinLength(16)]
        public string appId { get; set; }

        [Required, MaxLength(32)]
        public string env { get; set; }

        [MaxLength(32)]
        public string config { get; set; }
    }
}
