using System.Collections.Generic;
using System.Threading;

namespace AntJoin.Redis.Builders
{
    internal class RedisClientPools
    {
        private static readonly SemaphoreSlim RedisConnectionLock = new SemaphoreSlim(1, 1);
        private static readonly IDictionary<string, IRedisClient> RedisPools = new Dictionary<string, IRedisClient>();


        /// <summary>
        /// 获取一个客户端
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static IRedisClient Get(string name)
        {
            var key = Fingerprint.ToMd5Fingerprint(name);
            return !RedisPools.ContainsKey(key) ? null : RedisPools[key];
        }


        /// <summary>
        /// 添加客户端
        /// </summary>
        /// <param name="name"></param>
        /// <param name="client"></param>
        internal static void Add(string name, IRedisClient client)
        {
            RedisConnectionLock.Wait();
            try
            {
                var key = Fingerprint.ToMd5Fingerprint(name);
                if (!RedisPools.ContainsKey(key))
                {
                    RedisPools[key] = client;
                }
            }
            finally
            {
                RedisConnectionLock.Release();
            }
        }
    }
}
