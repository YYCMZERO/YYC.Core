using System;
using AntJoin.MQ.Options;

namespace AntJoin.MQ.Contracts
{
    /// <summary>
    /// Mq通道
    /// </summary>
    public interface IRabbitMqChannel : IDisposable
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTrans();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTrans();
        
        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTrans();

        /// <summary>
        /// 定义路由，没有定义路由无法创建生产者和消费者
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        IRabbitMqChannel DefinedRoute(MqExchangeOption option);

        
        /// <summary>
        /// 定义路由，没有定义路由无法创建生产者和消费者
        /// </summary>
        /// <param name="name">路由名称，RabbitMQ通过路由来转发消息</param>
        /// <param name="type">路由类型</param>
        /// <returns></returns>
        IRabbitMqChannel DefinedRoute(string name, string type = null);

        /// <summary>
        /// 创建发布者，创建发布者之前要定义好路由
        /// </summary>
        /// <returns></returns>
        IRabbitMqProducer CreateProducer();

        /// <summary>
        /// 绑定队列，绑定队列前，需要定义路由，否则队列无法绑定
        /// 因为RabbitMQ中队列是必须跟路由绑定的，不能单独存在
        /// </summary>
        /// <param name="queueOption"></param>
        /// <returns></returns>
        IRabbitMqChannel BindQueue(MqQueueOption queueOption);


        /// <summary>
        /// 绑定队列，绑定队列前，需要定义路由，否则队列无法绑定
        /// 因为RabbitMQ中队列是必须跟路由绑定的，不能单独存在
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        IRabbitMqChannel BindQueue(string queueName);

        /// <summary>
        /// 绑定队列路由键
        /// </summary>
        /// <param name="routeKeyList"></param>
        /// <returns></returns>
        IRabbitMqChannel BindRouteKey(string[] routeKeyList);

        /// <summary>
        /// 创建消费者，创建消费者之前必须定义好路由和队列
        /// </summary>
        /// <returns></returns>
        IRabbitMqConsumer CreateConsumer();
    }
}