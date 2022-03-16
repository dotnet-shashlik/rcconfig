namespace Shashlik.RC.Server.Secret.GrantTypes;

/// <summary>
/// 服务策略
/// </summary>
public interface IServiceStrategy
{
    /// <summary>
    /// 策略名称
    /// </summary>
    public string Strategy { get; }
}