using System;
using AntJoin.MQ.EventHandlers;

namespace AntJoin.MQ.EventBus
{
    public class DefaultEventNameProvider : IEventNameProvider
    {
        public string GetEventName<TEvent>() where TEvent : IntegratedEvent
        {
            return GetEventName(typeof(TEvent));
        }

        public string GetEventName(Type eventType) => eventType.FullName;
    }
}