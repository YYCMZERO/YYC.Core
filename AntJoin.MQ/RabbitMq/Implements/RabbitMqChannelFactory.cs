using AntJoin.MQ.Contracts;
using AntJoin.MQ.Options;
using Microsoft.Extensions.Logging;

namespace AntJoin.MQ.Implements
{
    public class RabbitMqChannelFactory : IRabbitMqChannelFactory
    {
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RabbitMqChannelFactory> _logger;

        /// <summary>
        /// 初始化工厂
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="loggerFactory"></param>
        public RabbitMqChannelFactory(IRabbitMqConnectionFactory connectionFactory,ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<RabbitMqChannelFactory>();
        }

        public IRabbitMqChannel Create(string connectionName = null)
        {
            var option = new MqConnectionOption(connectionName);
            return Create(option);
        }

        public IRabbitMqChannel Create(MqConnectionOption option)
        {
            var channel = new RabbitMqChannel(_connectionFactory, _loggerFactory, option);
            return channel;
        }
    }
}                                                                  