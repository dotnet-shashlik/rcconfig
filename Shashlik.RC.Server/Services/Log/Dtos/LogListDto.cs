#nullable disable
namespace Shashlik.RC.Server.Services.Log.Dtos;

public class LogListDto
{
    public long Id { get; set; }

    public long LogTime { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public string LogType { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    /// <summary>
    /// 日志概要信息
    /// </summary>
    public string Description { get; set; }

    public int FileId { get; set; }

    public string FileName { get; set; }

    public string ResourceId { get; set; }

    public string BeforeContent { get; set; }

    public string AfterContent { get; set; }
}