namespace AntJoin.MQ.Options
{
    /// <summary>
    /// 消息队列连接端点配置
    /// </summary>
    public class MqEndPoint
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 证书
        /// </summary>
        public MqSslOption Ssl { get; set; }


        /// <summary>
        /// 初始化
        /// </summary>
        public MqEndPoint() : this("localhost", 5672)
        {

        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public MqEndPoint(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}