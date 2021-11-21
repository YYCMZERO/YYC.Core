using System;
using System.Collections.Generic;
using System.Linq;
using AntJoin.MQ.EventHandlers;

namespace AntJoin.MQ.EventBus
{
    public class DefaultEventSubscriptionsManager : IEventSubscriptionsManager
    {
        private readonly Dictionary<string, List<Subscription>> _subscriptions;
        private readonly IEventNameProvider _eventNameProvider;


        public DefaultEventSubscriptionsManager(IEventNameProvider eventNameProvider)
        {
            _subscriptions = new Dictionary<string, List<Subscription>>();
            _eventNameProvider = eventNameProvider;
        }

        public event EventHandler<string> OnSubscribeRemove;


        public int Add<TEvent, THandler>() where TEvent : IntegratedEvent
            where THandler : IIntegrateEventHandler<TEvent>
        {
            var eventName = _eventNameProvider.GetEventName<TEvent>();
            var eventType = typeof(TEvent);
            var handlerType = typeof(THandler);

            if (!_subscriptions.ContainsKey(eventName))
            {
                _subscriptions.Add(eventName, new List<Subscription>
                {
                    new Subscription
                    {
                        Event = eventType,
                        EventHandler = handlerType
                    }
                });

                return 1;
            }

            if (_subscriptions[eventName].All(s => s.EventHandler != handlerType))
            {
                _subscriptions[eventName].Add(new Subscription
                {
                    Event = eventType,
                    EventHandler = handlerType
                });
            }
            return _subscriptions[eventName].Count();
        }

        public void Remove<TEvent, THandler>() where TEvent : IntegratedEvent
            where THandler : IIntegrateEventHandler<TEvent>
        {
            Remove(typeof(TEvent), typeof(THandler));
        }

        public void Remove(Type eventType, Type handlerType)
        {
            var eventName = _eventNameProvider.GetEventName(eventType);
            if (_subscriptions.ContainsKey(eventName))
            {
                _subscriptions[eventName].RemoveAll(s => s.EventHandler == handlerType);
                if (!_subscriptions[eventName].Any())
                {
                    _subscriptions.Remove(eventName);
                }

                RaiseOnEventRemoved(eventName);
            }
        }

        public Subscription[] Get(string eventName)
        {
            return _subscriptions.ContainsKey(eventName) ? _subscriptions[eventName].ToArray() : new Subscription[0];
        }
        public Subscription[] GetAll() => _subscriptions.SelectMany(s => s.Value).ToArray();

        public bool IsSubscribeEvent(string eventName) => _subscriptions.ContainsKey(eventName);

        public void Dispose() => _subscriptions.Clear();


        private void RaiseOnEventRemoved(string eventName)
        {
            if (!_subscriptions.ContainsKey(eventName))
            {
                OnSubscribeRemove?.Invoke(this, eventName);
            }
        }
    }
}