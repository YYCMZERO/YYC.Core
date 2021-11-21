namespace AntJoin.Dapper.Cache
{
    /// <summary>
    /// 二级缓存配置
    /// </summary>
    public class L2CacheOption
    {
        /// <summary>
        /// 主机密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 默认连接数据库
        /// </summary>
        public int DefaultDb { get; set; }

        /// <summary>
        /// Key前缀，使用[:]去拼接，用途是用来区分不同系统的数据
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
