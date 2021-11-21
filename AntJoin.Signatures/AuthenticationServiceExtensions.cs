using AntJoin.Log;
using AntJoin.Redis;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AntJoin.Signatures
{
    public static class AuthenticationServiceExtensions
    {
        /// <summary>
        /// 添加IdentityServer4服务和自定义Token规则
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddIdentityServerWithCustomTokenRule<TValidator>(this IServiceCollection services, Action<AuthenticationServiceParameter> action)
            where TValidator : class, IResourceOwnerPasswordValidator
        {
            var parameter = new AuthenticationServiceParameter();
            action.Invoke(parameter);

            if (parameter.EnableEquipmentKicks && (string.IsNullOrWhiteSpace(parameter.CacheConnection.Host) || parameter.CacheConnection.Port == 0))
            {
                LogHelper.Log(LogLevel.ERROR, "启动设备互踢 必须配置Redis缓存");
                throw new ArgumentNullException("启动设备互踢 必须配置Redis缓存");
            }
     
            var builder = services.AddIdentityServer(parameter.IdentityServerOption)
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<TValidator>();

            services.Add(ServiceDescriptor.Transient<ITokenClearService, TokenClearFromRedisService>());
            if (parameter.EnableEquipmentKicks)
            {
                services.Add(ServiceDescriptor.Transient<ITokenCreationService, DefaultTokenCreationAndSaveToRedisService>());
                services.Add(ServiceDescriptor.Transient<IRefreshTokenService, DefaultRefreshTokenSaveOneService>());

                services.AddRedis(options =>
                {
                    options.DefaultDb = parameter.CacheConnection.DbName;
                    options.Password = parameter.CacheConnection.Password;
                    options.RedisClientName = SignatureConstants.RedisClientName;
                    options.EndPoints.Add(new ServerEndPoint(parameter.CacheConnection.Host, parameter.CacheConnection.Port));
                    options.KeyPrefix = parameter.CacheConnection.KeyPrefix ?? SignatureConstants.DbKeyPrefix;
                });
            }

            services.AddAuthentication();
            services.AddAuthorization();
            return builder;
        }
    }
}
