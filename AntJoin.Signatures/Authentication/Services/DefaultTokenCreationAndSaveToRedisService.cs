using AntJoin.Core.Domains;
using AntJoin.Redis;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace AntJoin.Signatures
{
    /// <summary>
    /// 默认创建Token服务，会把Token保存到Redis上
    /// </summary>
    public class DefaultTokenCreationAndSaveToRedisService : DefaultTokenCreationService
    {
        protected readonly IRedisClientProvider RedisClientProvider;
        protected readonly IRefreshTokenStore RefreshTokenStore;

        public DefaultTokenCreationAndSaveToRedisService(ISystemClock clock,
            IKeyMaterialService keys,
            IdentityServerOptions options,
            ILogger<DefaultTokenCreationService> logger,
            IRefreshTokenStore refreshTokenStore,
            IRedisClientProvider redisClientProvider)
        : base(clock, keys, options, logger)
        {
            RedisClientProvider = redisClientProvider;
            RefreshTokenStore = refreshTokenStore;
        }


        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async override Task<string> CreateTokenAsync(Token token)
        {
            var strToken = await base.CreateTokenAsync(token);
            if (token.Type == TokenTypes.AccessToken)
            {
                var redisClient = RedisClientProvider.Get(SignatureConstants.RedisClientName);
                var key = $"{token.ClientId}:{token.SubjectId}_token";
                await redisClient.StringSet(key, strToken, TimeSpan.FromSeconds(token.Lifetime));
            }
            return strToken;
        }
    }
}
