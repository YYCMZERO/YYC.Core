using AntJoin.Dapper.Cache;
using AntJoin.Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DapperExtensions
    {
        public static IServiceCollection AddDapperL2Cache(this IServiceCollection services, Action<L2CacheOption> action)
        {
            var option = new L2CacheOption();
            action?.Invoke(option);
            if (!string.IsNullOrWhiteSpace(option.Host))
            {
                services.AddRedis(o =>
                {
                    o.DefaultDb = option.DefaultDb;
                    o.KeyPrefix = option.KeyPrefix;
                    o.Password = option.Password;
                    o.RedisClientName = DapperConstants.L2CacheKey;
                    o.EndPoints.Add(new ServerEndPoint(option.Host, option.Port));
                });
                
                using var sp = services.BuildServiceProvider();
                var redisClientProvider = sp.GetRequiredService<IRedisClientProvider>();
                L2CacheProvider.Set(redisClientProvider.Get(DapperConstants.L2CacheKey));
            }
            return services;
        }
    }
}
