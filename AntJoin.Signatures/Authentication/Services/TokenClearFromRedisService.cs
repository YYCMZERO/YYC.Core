using AntJoin.Core.Domains;
using AntJoin.Redis;
using IdentityServer4.Stores;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AntJoin.Signatures
{
    public class TokenClearFromRedisService : ITokenClearService
    {
        private readonly IRedisClientProvider _redisClientProvider;
        private readonly JwtSecurityTokenHandler _securityTokenHandler;
        private readonly IRefreshTokenStore _refreshTokenStore;

        public TokenClearFromRedisService(IRedisClientProvider redisClientProvider,
            IRefreshTokenStore refreshTokenStore)
        {
            _redisClientProvider = redisClientProvider;
            _securityTokenHandler = new JwtSecurityTokenHandler();
            _refreshTokenStore = refreshTokenStore;
        }


        /// <summary>
        /// 清除Token
        /// </summary>
        /// <param name="clearToken"></param>
        /// <returns></returns>
        public async Task<bool> ClearToken(string token)
        {
            if (_securityTokenHandler.CanReadToken(token))
            {
                var jwtToken = _securityTokenHandler.ReadJwtToken(token);
                var cid = jwtToken.Payload["client_id"];
                var key = $"{cid}:{jwtToken.Subject}_token";
                var redisClient = _redisClientProvider.Get(SignatureConstants.RedisClientName);

                if (!await redisClient.KeyExists(key)) return true;

                var accessToken = await redisClient.StringGet<string>(key);
                if (token == accessToken)
                {
                    await redisClient.KeyDelete(key);
                    await _refreshTokenStore.RemoveRefreshTokensAsync(jwtToken.Subject, cid as string);
                    return true;
                }
            }

            return false;
        }

    }
}
