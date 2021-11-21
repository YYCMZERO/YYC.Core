using System;
using System.Net.Sockets;
using AntJoin.MQ.Contracts;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace AntJoin.MQ.Implements
{
    public class DefaultConnectionStrategy : IConnectionStrategy
    {
        private readonly ILogger<DefaultConnectionStrategy> _logger;

        public DefaultConnectionStrategy(ILogger<DefaultConnectionStrategy> logger)
        {
            _logger = logger;
        }

        public ISyncPolicy CreatePolicy()
        {
            return Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(5,
                    d => TimeSpan.FromSeconds(Math.Pow(2, d)),
                    (ex, time) => { _logger?.LogError(ex, $"RabbitMq连接服务失败，在尝试 {time.TotalSeconds}s 后无法连接"); });
        }
    }
}