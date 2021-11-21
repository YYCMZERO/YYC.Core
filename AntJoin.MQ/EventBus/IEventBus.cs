using System.Threading.Tasks;
using AntJoin.MQ.EventHandlers;

namespace AntJoin.MQ.EventBus
{
    /// <summary>
    /// RabbitMQ消息总线
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="event"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task Publish<TEvent>(TEvent @event) where TEvent : IntegratedEvent;


        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        /// <returns></returns>
        Task Subscribe<TEvent, TEventHandler>() where TEvent : IntegratedEvent
            where TEventHandler : IIntegrateEventHandler<TEvent>;


        /// <summary>
        /// 取消对应订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        /// <returns></returns>
        Task UnSubscribe<TEvent, TEventHandler>() where TEvent : IntegratedEvent
            where TEventHandler : IIntegrateEventHandler<TEvent>;


        /// <summary>
        /// 取消该事件的所有订阅
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task UnSubscribe<TEvent>() where TEvent : IntegratedEvent;


        /// <summary>
        /// 取消所有的订阅
        /// </summary>
        /// <returns></returns>
        Task UnSubscribeAll();
    }
}