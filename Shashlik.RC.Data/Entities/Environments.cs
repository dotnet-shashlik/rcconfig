using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace Shashlik.RC.Data.Entities;

[Index("uk_Environments_ResourceId", "ResourceId", true)]
public class Environments : IResource
{
    /// <summary>
    /// id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; set; }

    /// <summary>
    /// ip白名单
    /// </summary>
    public string IpWhites { get; set; }

    /// <summary>
    /// 应用id
    /// </summary>
    public int ApplicationId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreateTime { get; set; }

    /// <summary>
    /// 最后修改时间,包含环境中的文件
    /// </summary>
    public long Version { get; set; }

    public Applications Application { get; set; }

    public List<Files> Files { get; set; }

    public List<Secrets> Secrets { get; set; }

    /// <summary>
    /// 资源id
    /// </summary>
    [Required]
    public string ResourceId { get; set; }
}