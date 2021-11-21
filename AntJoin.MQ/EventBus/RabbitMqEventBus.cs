using System.Text;
using System.Threading.Tasks;
using AntJoin.MQ.Contracts;
using AntJoin.MQ.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.RabbitMq.Exceptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AntJoin.MQ.EventBus
{
    public class RabbitMqEventBus : IEventBus
    {
        private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;
        private readonly IEventSubscriptionsManager _eventSubscriptionsManager;
        private readonly IEventNameProvider _eventNameProvider;
        private readonly RabbitMqEventBusOption _option;
        protected IRabbitMqConsumer Consumer;
        protected IRabbitMqProducer Producer;
        protected readonly IServiceScopeFactory ScopeFactory;

        public RabbitMqEventBus(IRabbitMqChannelFactory rabbitMqChannelFactory,
            IEventSubscriptionsManager eventSubscriptionsManager,
            IEventNameProvider eventNameProvider,
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqEventBusOption> options)
        {
            _rabbitMqChannelFactory = rabbitMqChannelFactory;
            _eventSubscriptionsManager = eventSubscriptionsManager;
            _eventNameProvider = eventNameProvider;
            _option = options.Value;
            ScopeFactory = scopeFactory;
            Initialize();
        }


        private void Initialize()
        {
            Producer = _rabbitMqChannelFactory
                .Create(_option.Connection)
                .DefinedRoute(_option.Exchange)
                .BindQueue(_option.Queue)
                .CreateProducer();
            
            Consumer = _rabbitMqChannelFactory
                .Create(_option.Connection)
                .DefinedRoute(_option.Exchange)
                .BindQueue(_option.Queue)
                .CreateConsumer();

            Consumer.OnReceived(ProcessEvent);       
            _eventSubscriptionsManager.OnSubscribeRemove += RemoveBindKey;
        }

        private void RemoveBindKey(object sender, string e) => Consumer.UnBindKey(e);
        
        
        private async Task ProcessEvent(IModel channel, BasicDeliverEventArgs args)
        {
            var eventName = args.RoutingKey;
            if (!_eventSubscriptionsManager.IsSubscribeEvent(eventName))
            {
                throw new NotFoundEventHandlerException("没有找到事件处理");
            }

            var subscriptions = _eventSubscriptionsManager.Get(eventName);
            using var scope = ScopeFactory.CreateScope();

            for (var i = 0; i < subscriptions.Length; i++)
            {
                var handler = scope.ServiceProvider.GetService(subscriptions[i].EventHandler);
                if (handler == null)
                {
                    continue;
                }

                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                var intergrationEvent = JsonConvert.DeserializeObject(message, subscriptions[i].Event);
                var concreteType = typeof(IIntegrateEventHandler<>).MakeGenericType(subscriptions[i].Event);
                await Task.Yield();
                await (Task)concreteType.GetMethod("Do")?.Invoke(handler, new[] {intergrationEvent});
            }
        }
        
        
        public async Task Publish<TEvent>(TEvent @event) where TEvent : IntegratedEvent
        {
            var routeKey = typeof(TEvent).FullName;
            var message = JsonConvert.SerializeObject(@event);
            var data = Encoding.UTF8.GetBytes(message);

            Producer.Send(routeKey, data);
            await Task.CompletedTask;
        }


        public async Task Subscribe<TEvent, TEventHandler>() where TEvent : IntegratedEvent
            where TEventHandler : IIntegrateEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).FullName;
            if (_eventSubscriptionsManager.Add<TEvent, TEventHandler>() == 1)
            {
                Consumer.BindKey(eventName);
            }
            await Task.CompletedTask;
        }
        
        
        public async Task UnSubscribe<TEvent, TEventHandler>() where TEvent : IntegratedEvent
            where TEventHandler : IIntegrateEventHandler<TEvent>
        {
            _eventSubscriptionsManager.Remove<TEvent, TEventHandler>();
            await Task.Yield();
        }

        
        public async Task UnSubscribe<TEvent>() where TEvent : IntegratedEvent
        {
            var eventName = _eventNameProvider.GetEventName<TEvent>();
            var subscriptions = _eventSubscriptionsManager.Get(eventName);
            for (var i = 0; i < subscriptions.Length; i++)
            {
                _eventSubscriptionsManager.Remove(subscriptions[i].Event, subscriptions[i].EventHandler);
            }
            await Task.Yield();
        }

        
        public async Task UnSubscribeAll()
        {
            var subscriptions = _eventSubscriptionsManager.GetAll();
            for (var i = 0; i < subscriptions.Length; i++)
            {
                _eventSubscriptionsManager.Remove(subscriptions[i].Event, subscriptions[i].EventHandler);
            }
            await Task.Yield();
        }
    }
}