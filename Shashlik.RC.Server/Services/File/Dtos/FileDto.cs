#nullable disable
namespace Shashlik.RC.Server.Services.File.Dtos;

public class FileDto
{
    /// <summary>
    /// 文件id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; set; }

    /// <summary>
    /// 文件类型,yaml/json
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 文件内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 环境id
    /// </summary>
    public int EnvironmentId { get; set; }

    /// <summary>
    /// 环境 name
    /// </summary>
    public string EnvironmentResourceId { get; set; }
}