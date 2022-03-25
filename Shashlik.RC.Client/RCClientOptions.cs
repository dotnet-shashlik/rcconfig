namespace Shashlik.RC.Client;

public class RCClientOptions
{
    /// <summary>
    /// 服务 api获取地址
    /// </summary>
    public string Server { get; set; } = null!;

    /// <summary>
    /// appId
    /// </summary>
    public string SecretId { get; set; } = null!;

    /// <summary>
    /// app key
    /// </summary>
    public string SecretKey { get; set; } = null!;

    /// <summary>
    /// 资源id
    /// </summary>
    public string ResourceId { get; set; } = null!;

    /// <summary>
    /// 轮询间隔, 单位秒, 0:不轮询
    /// </summary>
    public int Timeout { get; set; } = 29;
}