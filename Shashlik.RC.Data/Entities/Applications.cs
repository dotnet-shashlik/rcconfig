using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace Shashlik.RC.Data.Entities;

[Index("uk_Applications_ResourceId", "ResourceId", true)]
public class Applications : IResource
{
    public int Id { get; set; }

    /// <summary>
    /// app名称,不可修改
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// app描述
    /// </summary>
    public string Desc { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreateTime { get; set; }

    public List<Environments> Environments { get; set; }

    /// <summary>
    /// 资源id
    /// </summary>
    [Required]
    public string ResourceId { get; set; }
}