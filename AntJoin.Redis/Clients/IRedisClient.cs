namespace AntJoin.Redis
{
    /// <summary>
    /// 缓存客户端
    /// </summary>
    public interface IRedisClient : IRedisKeyFunc, IRedisStringFunc, IRedisListFunc, IRedisSetFunc, IRedisSortedSetFunc, IRedisLockFunc, IRedisHashFunc, IRedisSubscribe, IRedisTransaction
    {
        /// <summary>
        /// 切换数据库，对于Redis集群不能使用，
        /// 因为Redis集群不支持多库，
        /// Redis集群只有一个库，就是0库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public IRedisClient SelectDb(int dbName = -1);


        /// <summary>
        /// 重置数据库，重置为配置参数默认的数据库
        /// </summary>
        /// <returns></returns>
        public IRedisClient ResetDb();
    }
}
