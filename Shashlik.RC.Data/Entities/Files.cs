using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace Shashlik.RC.Data.Entities;

[Index("uk_Files_EnvironmentResourceId_Name", "EnvironmentResourceId,Name", true)]
public class Files
{
    /// <summary>
    /// 文件id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; set; }

    /// <summary>
    /// 文件类型,yaml/json
    /// </summary>
    [Required]
    public string Type { get; set; }

    /// <summary>
    /// 文件内容
    /// </summary>
    [Required]
    public string Content { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreateTime { get; set; }

    /// <summary>
    /// 环境id
    /// </summary>
    public int EnvironmentId { get; set; }

    /// <summary>
    /// 所属环境
    /// </summary>
    public Environments Environment { get; set; }

    public string EnvironmentResourceId { get; set; }
}