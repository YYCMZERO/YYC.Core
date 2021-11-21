using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using AntJoin.Log;
using AntJoin.Redis;

namespace AntJoin.Signatures
{
    /// <summary>
    /// 签名验证处理
    /// </summary>
    public class SignatureHandler : ISignatureHandler
    {
        /// <summary>
        /// 签名服务
        /// </summary>
        private readonly ISignatureService signatureService;
        private readonly IRedisClientProvider clientProvider;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="signatureService"></param>
        /// <param name="httpContextAccessor"></param>
        public SignatureHandler(ISignatureService signatureService, IRedisClientProvider clientProvider)
        {
            this.signatureService = signatureService;
            this.clientProvider = clientProvider;
        }


        /// <summary>
        /// 处理签名验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> HandleAsync(HttpContext context)
        {
            var clientId = context.Request.Headers["client_id"];
            if (string.IsNullOrWhiteSpace(clientId))
            {
                LogHelper.Log(Log.LogLevel.ERROR, "client_id is not exists");
                return false;
            }

            var secret = await GetSecretByClientId(clientId);
            if (secret == null)
            {
                LogHelper.Log(Log.LogLevel.ERROR, "can not find client secret");
                return false;
            }

            if (await signatureService.Validate(context, secret))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 根据客户端ID获取Client_Secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private async Task<string> GetSecretByClientId(string clientId)
        {
            var secret = string.Empty;
            var redisClient = clientProvider.Get(SignatureConstants.RedisClientName);
            if (await redisClient.HashExists(SignatureConstants.OAuthClientsSecretCacheKey, clientId))
            {
                secret = await redisClient.HashGet<string>(SignatureConstants.OAuthClientsSecretCacheKey, clientId);
            }
            else
            {
                LogHelper.Log(Log.LogLevel.ERROR, "找不到ClientId的信息，尝试到配置中心去同步操作");
            }
            return await Task.FromResult(secret);
        }
    }
}
