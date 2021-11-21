using System;
using AntJoin.MQ.Contracts;
using AntJoin.MQ.Options;
using RabbitMQ.Client;

namespace AntJoin.MQ.Implements
{
    public class RabbitMqProducer : IRabbitMqProducer
    {
        private IModel _channel;
        private readonly MqExchangeOption _exchangeOption;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeOption"></param>
        internal RabbitMqProducer(IModel channel, MqExchangeOption exchangeOption)
        {
            _channel = channel;
            _exchangeOption = exchangeOption;
        }

        public IRabbitMqProducer Send(string routeKey, byte[] message)
        {
            return Send(routeKey, message, null);
        }


        public IRabbitMqProducer Send(string routeKey, byte[] message, Action<IBasicProperties> configProperties)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = DeliveryModes.Persistent;

            configProperties?.Invoke(properties);
            routeKey ??= "";
            _channel.BasicPublish(_exchangeOption.Name, routeKey, false, properties, message);

            return this;
        }


        /// <summary>
        /// 释放生产者
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
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