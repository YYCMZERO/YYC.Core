namespace AntJoin.Signatures
{
    /// <summary>
    /// 缓存连接参数
    /// </summary>
    public class CacheConnectionParameter
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string RedisClientName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public int DbName { get; set; }

        /// <summary>
        ///前缀，用来区别数据库存储
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public CacheConnectionParameter()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="pwd"></param>
        /// <param name="db"></param>
        public CacheConnectionParameter(string host, int port, string pwd, int db)
        {
            Host = host;
            Port = port;
            Password = pwd;
            DbName = db;
        }
    }
}
