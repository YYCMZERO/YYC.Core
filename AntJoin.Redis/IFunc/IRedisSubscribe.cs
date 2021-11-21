using System;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    public interface IRedisSubscribe
    {
        /// <summary>
        /// 发布订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel">通道</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        Task<long> Publish<T>(string channel, T message);

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        Task Subscribe<T>(string channel, Action<string, T> handler);

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        Task Unsubscribe<T>(string channel, Action<string, T> handler = null);

        /// <summary>
        /// 取消所有订阅
        /// </summary>
        /// <returns></returns>
        Task UnsubscribeAll();
    }
}
