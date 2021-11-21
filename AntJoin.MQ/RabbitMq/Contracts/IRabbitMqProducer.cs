using RabbitMQ.Client;
using System;

namespace AntJoin.MQ.Contracts
{
    public interface IRabbitMqProducer : IDisposable
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="routeKey"></param>
        /// <param name="message"></param>
        IRabbitMqProducer Send(string routeKey, byte[] message);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="routeKey"></param>
        /// <param name="message"></param>
        /// <param name="configProperties"></param>
        /// <returns></returns>
        IRabbitMqProducer Send(string routeKey, byte[] message, Action<IBasicProperties> configProperties);

        /// <summary>
        /// 释放生产者，关闭channel通道
        /// </summary>
        void Close();
    }
}                                 