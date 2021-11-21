using System.Threading.Tasks;

namespace AntJoin.MQ.EventHandlers
{
    /// <summary>
    ///  事件处理器
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IIntegrateEventHandler<in TEvent> : IEventHandler where TEvent : IntegratedEvent
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task Do(TEvent @event);
    }
}