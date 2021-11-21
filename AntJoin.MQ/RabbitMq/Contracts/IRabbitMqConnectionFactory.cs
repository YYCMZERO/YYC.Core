using AntJoin.MQ.Models;
using AntJoin.MQ.Options;

namespace AntJoin.MQ.Contracts
{
    /// <summary>
    ///  RabbitMQ连接创建工厂
    /// </summary>
    public interface IRabbitMqConnectionFactory
    {
        /// <summary>
        /// 创建连接或者获取，如果连接已经存在则获取连接
        /// </summary>
        /// <param name="option">如果连接名不存在，则会根据这个option来创建连接</param>
        /// <returns></returns>
        RabbitMqConnection GetOrCreate(MqConnectionOption option);
    }         
}