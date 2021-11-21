using AntJoin.Log;
using AntJoin.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace AntJoin.Signatures
{
    /// <summary>
    /// 客户端认证服务
    /// </summary>
    public static class AnthenticationClientExtensions
    {
        /// <summary>
        /// 添加认证客户端服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthenticationClientService(this IServiceCollection services, Action<AuthenticationClientParameter> action)
        {
            var parameter = new AuthenticationClientParameter();
            action.Invoke(parameter);

            if ((parameter.EnableEquipmentKicks || parameter.EnabledSignature) && (string.IsNullOrWhiteSpace(parameter.CacheConnection.Host) || parameter.CacheConnection.Port == 0))
            {
                LogHelper.Log(LogLevel.ERROR, "启动设备互踢 必须配置Redis缓存");
                throw new ArgumentNullException("启动设备互踢 或者 签名服务 必须配置Redis缓存");
            }

            services.AddRedis(options =>
            {
                options.DefaultDb = parameter.CacheConnection.DbName;
                options.Password = parameter.CacheConnection.Password;
                options.RedisClientName = parameter.CacheConnection.RedisClientName ?? SignatureConstants.RedisClientName;
                options.EndPoints.Add(new ServerEndPoint(parameter.CacheConnection.Host, parameter.CacheConnection.Port));
                options.KeyPrefix = parameter.CacheConnection.KeyPrefix ?? SignatureConstants.DbKeyPrefix;

            });

            if (parameter.EnabledSignature)
            {
                services.AddScoped<ISignatureService, TokenSignatureBaseRedisService>();
                services.AddScoped<ISignatureBuilder, Md5SignatureBuilder>();
                services.AddScoped<ISignatureHandler, SignatureHandler>();
                services.AddTransient<SignatureAttribute>();
                services.AddMvcCore(option => option.Filters.Add<SignatureAttribute>());
            }

            JwtSecurityTokenHandler.DefaultInboundClaimFilter.Clear();
            var builder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            if (parameter.EnableEquipmentKicks)
            {
                builder.AddJwtTokenAuthentication(parameter.JwtOptions);
            }
            else
            {
                builder.AddJwtTokenAuthentication<JwtBearerHandler>(parameter.JwtOptions);
            }

            return services;
        }
    }
}
