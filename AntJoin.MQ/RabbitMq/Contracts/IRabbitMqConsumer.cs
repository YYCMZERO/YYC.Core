using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AntJoin.MQ.Contracts
{
    public interface IRabbitMqConsumer :IRouteKeyBinder, IDisposable
    {
        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="doFunc"></param>
        IRabbitMqConsumer OnReceived(Func<IModel, BasicDeliverEventArgs, Task> doFunc);
    }
}