using System;
using System.Collections.Generic;
using AntJoin.MQ.EventHandlers;

namespace AntJoin.MQ.EventBus
{
    /// <summary>
    /// 事件订阅管理器
    /// </summary>
    public interface IEventSubscriptionsManager : IDisposable
    {
        /// <summary>
        /// 移除订阅触发
        /// </summary>
        event EventHandler<string> OnSubscribeRemove;

        /// <summary>
        /// 添加订阅信息
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        int Add<TEvent, THandler>() where TEvent : IntegratedEvent where THandler : IIntegrateEventHandler<TEvent>;


        /// <summary>
        /// 移除订阅信息
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        void Remove<TEvent, THandler>() where TEvent : IntegratedEvent where THandler : IIntegrateEventHandler<TEvent>;

        /// <summary>
        /// 移除订阅信息
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handlerType"></param>
        void Remove(Type eventType, Type handlerType);


        /// <summary>
        /// 获取对应主题事件的订阅内容
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Subscription[] Get(string eventName);

        /// <summary>
        /// 获取所有订阅内容
        /// </summary>
        /// <returns></returns>
        Subscription[] GetAll();

        /// <summary>
        /// 是否有订阅过该事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool IsSubscribeEvent(string eventName);
    }
}