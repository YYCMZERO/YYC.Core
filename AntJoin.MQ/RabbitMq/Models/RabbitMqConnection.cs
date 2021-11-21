using AntJoin.MQ.Options;
using RabbitMQ.Client;

namespace AntJoin.MQ.Models
{
    /// <summary>
    /// RabbitMQ 连接
    /// </summary>
    public class RabbitMqConnection
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectionName { get; set; }
        
        /// <summary>
        /// 连接参数
        /// </summary>
        public MqConnectionOption ConnectionOption { get; set; }
        
        /// <summary>
        /// 连接对象
        /// </summary>
        public IConnection Connection { get; set; }

        
        public RabbitMqConnection(MqConnectionOption connectionOption)
        {
            ConnectionName = connectionOption.ConnectionName;
            ConnectionOption = connectionOption;
        }
    }
}