using AntJoin.MQ.Options;

namespace AntJoin.MQ.Contracts
{
    /// <summary>
    /// 通道工厂
    /// </summary>
    public interface IRabbitMqChannelFactory
    {
        /// <summary>
        /// 创建通道
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        IRabbitMqChannel Create(string connectionName = null);

        /// <summary>
        /// 创建通道
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        IRabbitMqChannel Create(MqConnectionOption option);
    }
}