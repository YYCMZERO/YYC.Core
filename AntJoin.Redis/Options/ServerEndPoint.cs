namespace AntJoin.Redis
{
    /// <summary>
    /// 服务器端点
    /// </summary>
    public class ServerEndPoint
    {
        /// <summary>
        /// 获取或设置 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 获取或设置 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 初始化一个<see cref="ServerEndPoint"/>类型的实例
        /// </summary>
        public ServerEndPoint()
        {
        }

        /// <summary>
        /// 初始化一个<see cref="ServerEndPoint"/>类型的实例
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口号</param>
        public ServerEndPoint(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
