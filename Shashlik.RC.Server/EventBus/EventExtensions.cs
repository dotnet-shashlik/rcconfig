using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.RC.Server.EventBus;

public static class EventExtensions
{
    /// <summary>
    /// 同步分发事件,基于内存
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="eventName"></param>
    /// <param name="eventData"></param>
    public static void Dispatch(this IServiceProvider serviceProvider, string eventName, object? eventData)
    {
        foreach (var item in serviceProvider.GetRequiredService<IEnumerable<IEventHandler>>())
        {
            if (item.EventName == eventName)
                item.Process(eventData);
        }
    }
}