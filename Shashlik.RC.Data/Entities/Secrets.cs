using System.ComponentModel.DataAnnotations;

namespace Shashlik.RC.Data.Entities;

public class Secrets
{
    public int Id { get; set; }

    [Required] public string SecretId { get; set; }
    [Required] public string SecretKey { get; set; }

    public long CreateTime { get; set; }
    [Required] public string UserId { get; set; }
}