using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AntJoin.Redis
{
    /// <summary>
    /// 连接参数
    /// </summary>
    public class ConnectionOption
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public ConnectionOption()
        {
            EndPoints = new List<ServerEndPoint>();
            DefaultDb = 0;
            KeyPrefix = "";
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="redisClientName">客户端名称</param>
        /// <param name="host">服务器地址</param>
        /// <param name="port">端口</param>
        /// <param name="password">密码</param>
        /// <param name="defaultDb">数据库</param>
        /// <param name="keyPrefix">KEY前缀</param>
        public ConnectionOption([NotNull] string redisClientName,[NotNull] string host, int port = 6379, string password = null, int defaultDb = 0, string keyPrefix = "")
        {
            EndPoints = new List<ServerEndPoint>
            {
                new ServerEndPoint(host, port)
            };
            Password = password;
            DefaultDb = defaultDb;
            KeyPrefix = keyPrefix;
            RedisClientName = redisClientName;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="redisClientName">客户端名称</param>
        /// <param name="endpoints">地址池</param>
        /// <param name="password">密码</param>
        /// <param name="defaultDb">数据库</param>
        /// <param name="keyPrefix">KEY前缀</param>
        public ConnectionOption([NotNull] string redisClientName, [NotNull] List<ServerEndPoint> endpoints, string password = null, int defaultDb = 0, string keyPrefix = "")
        {
            EndPoints = endpoints;
            Password = password;
            DefaultDb = defaultDb;
            KeyPrefix = keyPrefix;
            RedisClientName = redisClientName;
        }

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
        /// 获取或设置 用于连接到Redis服务器的端点列表
        /// </summary>
        public IList<ServerEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Redis客户端名称
        /// </summary>
        public string RedisClientName { get; set; }


        /// <summary>
        /// 设置WorkCount的数量
        /// </summary>
        public int WorkerCount { get; set; } = 20;
    }
}
