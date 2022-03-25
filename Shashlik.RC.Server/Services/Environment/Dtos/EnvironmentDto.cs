#nullable disable
using Shashlik.RC.Data;

namespace Shashlik.RC.Server.Services.Environment.Dtos;

public class EnvironmentDto : IResource
{
    /// <summary>
    /// id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
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
    /// 应用name
    /// </summary>
    public string ApplicationName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreateTime { get; set; }

    /// <summary>
    /// 最后修改时间,包含环境中的文件
    /// </summary>
    public long Version { get; set; }

    /// <summary>
    /// 资源id
    /// </summary>
    public string ResourceId { get; set; }
}