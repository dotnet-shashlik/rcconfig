using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace Shashlik.RC.Data.Entities;

public class Secrets
{
    [Column(IsIdentity = true)] public int Id { get; set; }

    [Required] public string SecretId { get; set; }
    [Required] public string SecretKey { get; set; }

    public long CreateTime { get; set; }
    [Required] public string UserId { get; set; }
}