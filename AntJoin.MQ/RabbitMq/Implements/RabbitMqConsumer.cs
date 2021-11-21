using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using AntJoin.MQ.Contracts;
using AntJoin.MQ.Options;
using Microsoft.Extensions.DependencyInjection.RabbitMq.Exceptions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AntJoin.MQ.Implements
{
    public class RabbitMqConsumer : IRabbitMqConsumer
    {
        private IModel _channel;
        private readonly MqExchangeOption _exchangeOption;
        private readonly MqQueueOption _queueOption;
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>> _callbacks;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeOption"></param>
        /// <param name="queueOption"></param>
        /// <param name="loggerFactory"></param>
        internal RabbitMqConsumer(IModel channel, MqExchangeOption exchangeOption, MqQueueOption queueOption, ILoggerFactory loggerFactory)
        {
            _channel = channel;
            _exchangeOption = exchangeOption;
            _queueOption = queueOption;
            _callbacks = new ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>>();
            _logger = loggerFactory?.CreateLogger<RabbitMqConsumer>();

            StartConsume();
        }

        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="callback"></param>
        public IRabbitMqConsumer OnReceived(Func<IModel, BasicDeliverEventArgs, Task> callback)
        {
            _callbacks.Add(callback);
            return this;
        }

        /// <summary>
        /// 绑定路由键
        /// </summary>
        /// <param name="routeKey"></param>
        /// <returns></returns>
        public IRabbitMqConsumer BindKey(string routeKey)
        {
            routeKey ??= "";
            _channel.QueueBind(_queueOption.Name, _exchangeOption.Name, routeKey, _queueOption.Arguments);
            return this;
        }

        /// <summary>
        /// 解绑路由键
        /// </summary>
        /// <param name="routeKey"></param>
        /// <returns></returns>
        public IRabbitMqConsumer UnBindKey(string routeKey)
        {
            routeKey ??= "";
            _channel.QueueUnbind(_queueOption.Name, _exchangeOption.Name, routeKey, _queueOption.Arguments);
            return this;
        }

        private void StartConsume()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicQos(0, _queueOption.PrefetchCount, false);

            consumer.Received += async (sender, args) =>
            {
                try
                {
                    foreach (var callback in _callbacks)
                    {
                        await callback(_channel, args);
                    }

                    _channel.BasicAck(args.DeliveryTag, false);
                }
                catch (NotFoundEventHandlerException ex)
                {
                    //_channel.BasicReject(args.DeliveryTag, true);

                    // 转到死信队列
                    _channel.BasicNack(args.DeliveryTag, false, false);

                    _logger.LogWarning(ex.Message);
                }
                catch (Exception ex)
                {
                    //_channel.BasicReject(args.DeliveryTag, true);

                    // 转到死信队列
                    _channel.BasicNack(args.DeliveryTag, false, false);

                    var message = $"消息消费失败：[Exchange = {args.Exchange}] [RouteKey = {args.RoutingKey}] [Message = {Encoding.UTF8.GetString(args.Body.ToArray())}]";
                    _logger?.LogError(ex, message);
                }
            };
            _channel.BasicConsume(_queueOption.Name, false, consumer);
        }

        public void Dispose()
        {
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
                _channel.Dispose();
                _channel = null;
            }
        }
    }
}