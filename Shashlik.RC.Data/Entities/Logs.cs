using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace Shashlik.RC.Data.Entities;

/// <summary>
/// 日志记录
/// </summary>
public class Logs
{
    [Column(IsIdentity = true)] public long Id { get; set; }

    public long LogTime { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    [Required]
    public string LogType { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public int FileId { get; set; }

    public string FileName { get; set; }

    public string ResourceId { get; set; }

    [Column(StringLength = -1)] public string BeforeContent { get; set; }

    [Column(StringLength = -1)] public string AfterContent { get; set; }
}