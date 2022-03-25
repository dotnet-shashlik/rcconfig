namespace Shashlik.RC.Server.EventBus;

/// <summary>
/// 事件处理器
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; }

    /// <summary>
    /// OnExecuted
    /// </summary>
    /// <param name="eventData"></param>
    void Process(object? eventData);
}