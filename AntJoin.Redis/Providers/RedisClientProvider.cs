using AntJoin.Redis.Builders;
using Microsoft.Extensions.Logging;

namespace AntJoin.Redis
{
    /// <summary>
    /// Redis 数据库提供程序
    /// </summary>
    public class RedisClientProvider : IRedisClientProvider
    {
        private readonly IRedisClientBuilder _redisClientBuilder;
        private readonly ILogger _logger;


        /// <summary>
        /// 初始化Redis客户端提供者
        /// </summary>
        /// <param name="loggerFactory"></param>
        public RedisClientProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<RedisClientProvider>();
            _redisClientBuilder = new RedisClientBuilder(loggerFactory);
        }


        public void Add(ConnectionOption option)
        {
            var redisClientName = string.IsNullOrWhiteSpace(option.RedisClientName) ? Constants.DefaultRedisClientName : option.RedisClientName;
            RedisClientPools.Add(redisClientName, _redisClientBuilder.Build(option));
            _logger?.LogInformation($"添加了 关于客户端名称 {redisClientName} 对应的连接");
        }


        public IRedisClient Get(string redisClientName = null)
        {
            redisClientName = string.IsNullOrWhiteSpace(redisClientName) ? Constants.DefaultRedisClientName : redisClientName;
            var redisClient = RedisClientPools.Get(redisClientName);
            if (redisClient == null)
            {
                _logger?.LogInformation($"找不到了 关于客户端名称 {redisClientName} 对应的连接");
            }
            return redisClient;
        }
    }
}
