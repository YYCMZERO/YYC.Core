using System;
using System.Linq;
using AntJoin.MQ.Contracts;
using AntJoin.MQ.Models;
using AntJoin.MQ.Options;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AntJoin.MQ.Implements
{
    public class RabbitMqChannel : IRabbitMqChannel
    {
        private IModel _inner;
        private MqExchangeOption _exchangeOption;
        private MqQueueOption _queueOption;

        private readonly RabbitMqConnection _mqConnection;
        private readonly ILoggerFactory _loggerFactory;


        /// <summary>
        /// 初始化通道
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="option"></param>
        /// <param name="loggerFactory"></param>
        public RabbitMqChannel(IRabbitMqConnectionFactory connectionFactory,
            ILoggerFactory loggerFactory, MqConnectionOption option)
        {
            _mqConnection = connectionFactory.GetOrCreate(option);
            _loggerFactory = loggerFactory;
            Initialize();
        }




        #region 事务操作

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTrans() => _inner.TxSelect();

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTrans() => _inner.TxCommit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTrans() => _inner.TxRollback();

        #endregion

        #region 路由和队列定义

        /// <summary>
        /// 定义路由，没有定义路由无法创建生产者和消费者
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public IRabbitMqChannel DefinedRoute(MqExchangeOption option)
        {
            _exchangeOption = option;
            CreateExchange(_exchangeOption);
            return this;
        }

        /// <summary>
        /// 定义路由，没有定义路由无法创建生产者和消费者
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IRabbitMqChannel DefinedRoute(string name, string type = null)
        {
            type ??= MqExchangeType.Direct;
            var option = new MqExchangeOption(name, type);
            return DefinedRoute(option);
        }

        /// <summary>
        /// 绑定队列，绑定队列前，需要定义路由，否则队列无法绑定
        /// 因为RabbitMQ中队列是必须跟路由绑定的，不能单独存在
        /// </summary>
        /// <param name="queueOption"></param>
        /// <returns></returns>
        public IRabbitMqChannel BindQueue(MqQueueOption queueOption)
        {
            if (_exchangeOption == null)
            {
                throw new ArgumentNullException(nameof(_exchangeOption), "路由没有定义，无法绑定");
            }
            _queueOption = queueOption;
            CreateQueue(_queueOption);
            return this;
        }

        /// <summary>
        /// 绑定队列，绑定队列前，需要定义路由，否则队列无法绑定
        /// 因为RabbitMQ中队列是必须跟路由绑定的，不能单独存在
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public IRabbitMqChannel BindQueue(string queueName)
        {
            var option = new MqQueueOption(queueName);
            return BindQueue(option);
        }

        /// <summary>
        /// 绑定队列路由键
        /// </summary>
        /// <param name="routeKeyList"></param>
        /// <returns></returns>
        public IRabbitMqChannel BindRouteKey(string[] routeKeyList)
        {
            if (_exchangeOption == null)
            {
                throw new ArgumentNullException(nameof(_exchangeOption), "交换机没有定义，无法绑定");
            }
            if (_queueOption == null)
            {
                throw new ArgumentNullException(nameof(_queueOption), "队列没有定义，无法绑定");
            }
            CreateRouteKey(_exchangeOption, _queueOption, routeKeyList);
            return this;
        }
        #endregion

        #region 生产者和消费者

        /// <summary>
        /// 创建发布者，创建发布者之前要定义好路由
        /// </summary>
        /// <returns></returns>
        public IRabbitMqProducer CreateProducer()
        {
            if (_exchangeOption == null)
            {
                throw new ArgumentNullException(nameof(_exchangeOption), "路由没有定义，无法创建发布者");
            }
            return new RabbitMqProducer(_inner, _exchangeOption);
        }


        /// <summary>
        /// 创建订阅者，创建订阅者之前必须定义好路由和队列
        /// </summary>
        /// <returns></returns>
        public IRabbitMqConsumer CreateConsumer()
        {
            if (_exchangeOption == null)
            {
                throw new ArgumentNullException(nameof(_exchangeOption), "路由没有定义，无法创建订阅者");
            }

            if (_queueOption == null)
            {
                throw new ArgumentNullException(nameof(_queueOption), "路由，无法创建订阅者");
            }
            return new RabbitMqConsumer(_inner, _exchangeOption, _queueOption, _loggerFactory);
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() => _inner?.Dispose();

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            CreateModel();
        }

        /// <summary>
        /// 创建IModel
        /// </summary>
        private void CreateModel()
        {
            _inner = _mqConnection.Connection.CreateModel();
        }

        /// <summary>
        /// 创建路由器
        /// </summary>
        /// <param name="exchange"></param>
        private void CreateExchange(MqExchangeOption exchange)
        {
            _inner.ExchangeDeclare(
                exchange.Name,
                exchange.Type,
                exchange.Durable,
                exchange.AutoDelete,
                exchange.Arguments
            );
        }

        /// <summary>
        /// 创建队列
        /// </summary>
        /// <param name="queue"></param>
        private void CreateQueue(MqQueueOption queue)
        {
            _inner.QueueDeclare(
                queue.Name,
                queue.Durable,
                queue.Exclusive,
                queue.AutoDelete,
                queue.Arguments
            );
        }

        /// <summary>
        /// 绑定队列路由键
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="routeKeyList"></param>
        private void CreateRouteKey(MqExchangeOption exchange, MqQueueOption queue, string[] routeKeyList)
        {
            if (routeKeyList != null && routeKeyList.Any())
            {
                foreach (var routeKey in routeKeyList)
                {
                    _inner.QueueBind(queue.Name, exchange.Name, routeKey, queue.Arguments);
                }
            }
        }

        #endregion
    }
}