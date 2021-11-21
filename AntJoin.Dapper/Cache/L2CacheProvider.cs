using AntJoin.Redis;
using System;

namespace AntJoin.Dapper.Cache
{
    internal class L2CacheProvider
    {
        private static Lazy<IRedisClient> _cacheClient;


        /// <summary>
        /// 获取当前缓存客户端
        /// </summary>
        /// <returns></returns>
        internal static IRedisClient Get() => _cacheClient.Value;


        /// <summary>
        /// 设置客户端
        /// </summary>
        /// <param name="client"></param>
        internal static void Set(IRedisClient client)
        {
            _cacheClient = new Lazy<IRedisClient>(client);
        }
    }
}
