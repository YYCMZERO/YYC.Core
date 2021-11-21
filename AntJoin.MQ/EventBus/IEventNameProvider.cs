using System;
using AntJoin.MQ.EventHandlers;

namespace AntJoin.MQ.EventBus
{
    public interface IEventNameProvider
    {
        /// <summary>
        /// 获取事件名称
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        string GetEventName<TEvent>() where TEvent : IntegratedEvent;

        /// <summary>
        /// 获取事件名称
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        string GetEventName(Type eventType);
    }
}