using AntJoin.Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisExtensions
    {
        private static bool _isRunning = false;


        /// <summary>
        /// 添加Redis相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedis(this IServiceCollection services, params ConnectionOption[] connectionOptions)
        {
            if (!_isRunning)
            {
                services.Add(ServiceDescriptor.Singleton<IRedisClientProvider, RedisClientProvider>());
            }
            using (var sp = services.BuildServiceProvider())
            {
                var redisClientProvider = sp.GetRequiredService<IRedisClientProvider>();
                foreach (var option in connectionOptions)
                {
                    redisClientProvider.Add(option);
                }
            }
            _isRunning = true;
            return services;
        }


        /// <summary>
        /// 添加Redis相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionOptionAction">通过第二个参数来指定要添加那些Redis客户端</param>
        /// <returns></returns>
        public static IServiceCollection AddRedis(this IServiceCollection services,Action<ConnectionOption> connectionOptionAction)
        {
            if (!_isRunning)
            {
                services.Add(ServiceDescriptor.Singleton<IRedisClientProvider, RedisClientProvider>());   
            }
            var option = new ConnectionOption();
            connectionOptionAction.Invoke(option);
            using (var sp = services.BuildServiceProvider())
            {
                var redisClientProvider = sp.GetRequiredService<IRedisClientProvider>();
                redisClientProvider.Add(option);
            }
            _isRunning = true;
            return services;
        }
    }
}
