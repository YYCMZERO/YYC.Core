namespace AntJoin.Redis
{
    /// <summary>
    /// Redis 数据库提供程序
    /// </summary>
    public interface IRedisClientProvider
    {
        /// <summary>
        /// 获取指定的客户端，没有指定获取默认的客户端名称
        /// 默认客户端必须配置一个没有设置客户端名称的redis服务
        /// </summary>
        /// <param name="redisClientName">Redis客户端名称</param>
        /// <returns></returns>
        IRedisClient Get(string redisClientName = null);

        /// <summary>
        /// 添加redis服务，当<see cref="ConnectionOption.RedisClientName"/>为空时，会设置为默认客户端
        /// </summary>
        /// <param name="option"></param>
        void Add(ConnectionOption option);
    }
}
