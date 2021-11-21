using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using AntJoin.MQ.Contracts;
using AntJoin.MQ.Models;
using AntJoin.MQ.Options;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AntJoin.MQ.Implements
{
    /// <summary>
    /// 创建RabbitMQ连接工厂
    /// </summary>
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        private readonly ILogger<RabbitMqChannelFactory> _logger;
        private readonly IConnectionStrategy _strategy;

        /// <summary>
        /// 工厂初始化
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="strategy"></param>
        public RabbitMqConnectionFactory(ILoggerFactory loggerFactory,IConnectionStrategy strategy)
        {
            _logger = loggerFactory?.CreateLogger<RabbitMqChannelFactory>();
            _strategy = strategy;
        }


        /// <summary>                             
        /// 创建一个RabbitMQ连接
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public RabbitMqConnection GetOrCreate(MqConnectionOption option)
        {
            if (RabbitMqConnectionPool.Instance.TryGet(option.ConnectionName, out var mqConnection))
            {
                if (mqConnection.Connection == null)
                {
                    mqConnection.Connection = CreateConnection(mqConnection.ConnectionOption);
                    RabbitMqConnectionPool.Instance.Update(mqConnection.ConnectionName, mqConnection);
                }
            }
            else
            {
                ValidateArguments(option);
                mqConnection = new RabbitMqConnection(option);
                mqConnection.Connection = CreateConnection(mqConnection.ConnectionOption);
                _logger?.LogInformation($"成功创建RabbitMQ连接：[{option.ConnectionName}]");
                RabbitMqConnectionPool.Instance.Add(option.ConnectionName, mqConnection);
            }            
            
            return mqConnection;
        }


        /// <summary>
        /// 创建真实连接
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        private IConnection CreateConnection(MqConnectionOption option)
        {
            var endpoints = CreateEndpoints(option);
            var factory = new ConnectionFactory
            {
                UserName = option.UserName,
                Password = option.Password,
                AutomaticRecoveryEnabled = option.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = option.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = option.RequestedConnectionTimeout,
                RequestedHeartbeat = option.RequestedHeartbeat,
                DispatchConsumersAsync = true,
                VirtualHost = option.VirtualHost
            };

            IConnection connection = null;
            _strategy.CreatePolicy().Execute(() =>
            {
                connection = factory.CreateConnection(endpoints, option.ConnectionName);
                connection.CallbackException += (sender, args) =>
                {
                    if (args != null)
                    {
                        _logger?.LogError(new EventId(), args.Exception, $"回调异常：{args.Exception.Message}", args);
                    }
                };
            });

            return connection;
        }
        
        
        
        
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="option"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private void ValidateArguments(MqConnectionOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            if (!option.EndPoints.Any())
            {
                throw new ArgumentException("没有配置连接参数", nameof(option.EndPoints));
            }
        }
        
        /// <summary>
        /// 创建连接点
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        private IList<AmqpTcpEndpoint> CreateEndpoints(MqConnectionOption option)
        {
            var endpoints = new List<AmqpTcpEndpoint>();
            foreach (var ep in option.EndPoints)
            {
                if (ep.Ssl != null)
                {
                    var ssl = new SslOption(ep.Ssl.ServerName, ep.Ssl.CertPath)
                    {
                        CertPassphrase = ep.Ssl.CertPwd,
                        AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors |
                                                 SslPolicyErrors.RemoteCertificateNameMismatch
                    };
                    endpoints.Add(new AmqpTcpEndpoint(ep.Host, ep.Port, ssl));
                }
                else
                {
                    endpoints.Add(new AmqpTcpEndpoint(ep.Host, ep.Port));
                }
            }
            return endpoints;
        }
    }
}

