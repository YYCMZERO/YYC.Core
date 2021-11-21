using AntJoin.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AntJoin.Signatures
{
    public static class SignatureAnthorizeExtension
    {
        /// <summary>
        /// 添加签名服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionAction">签名Redis连接参数配置</param>
        /// <returns></returns>
        public static IServiceCollection AddSignatureService(this IServiceCollection services,Action<RedisConnectionOption> builder)
        {
            var option = new RedisConnectionOption();
            builder?.Invoke(option);

            //添加签名用的Redis服务器
            services.AddRedis(options =>
            {
                options.DefaultDb = option.RedisDbName;
                options.Password = option.RedisPassword;
                options.RedisClientName = option.RedisClientName ?? SignatureConstants.RedisClientName;
                options.EndPoints.Add(new ServerEndPoint(option.RedisHost, option.RedisPort));
                options.KeyPrefix = SignatureConstants.DbKeyPrefix;
            });

            services.AddScoped<ISignatureService, TokenSignatureBaseRedisService>();
            services.AddScoped<ISignatureBuilder, Md5SignatureBuilder>();
            services.AddScoped<ISignatureHandler, SignatureHandler>();
            services.AddTransient<SignatureAttribute>();
            services.AddMvcCore(option => option.Filters.Add<SignatureAttribute>());
            return services;
        }
    }
}
