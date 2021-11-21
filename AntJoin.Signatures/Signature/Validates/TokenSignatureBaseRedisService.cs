using AntJoin.Log;
using AntJoin.Redis;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntJoin.Signatures
{
    /// <summary>
    /// Token签名验证
    /// </summary>
    public class TokenSignatureBaseRedisService : ISignatureService
    {
        private readonly ISignatureBuilder _handler;
        private readonly List<string> skipKeys = new List<string>
        {
            "method",
            "timestamp",
            "client_id",
            "nonce",
            "signature"
        };
        private readonly IRedisClient redisClient;


        public TokenSignatureBaseRedisService(ISignatureBuilder handler, IRedisClientProvider clientProvider)
        {      
            _handler = handler;
            redisClient = clientProvider.Get(SignatureConstants.RedisClientName);
        }



        public async Task<bool> Validate(HttpContext context, string secret)
        {
            var signatureText = context.Request.Headers["signature"].ToString().ToLower();
            var timestamp = Convert.ToInt64(context.Request.Headers["timestamp"]);
            var client_id = context.Request.Headers["client_id"].ToString();
            var nonce = context.Request.Headers["nonce"].ToString();

            if (string.IsNullOrWhiteSpace(signatureText))
            {
                LogHelper.Log(LogLevel.INFO, "signature info is not transmit");
                return false;
            }

            if (timestamp == 0)
            {
                LogHelper.Log(LogLevel.INFO, "timestamp is not exists");
                return false;
            }

            if ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestamp) > TimeSpan.FromMinutes(5).TotalMilliseconds)
            {
                LogHelper.Log(LogLevel.INFO, "timestamp is expire");
                return false;
            }


            if (string.IsNullOrWhiteSpace(client_id))
            {
                LogHelper.Log(LogLevel.INFO, "client_id is not exists");
                return false;
            }

            if (string.IsNullOrWhiteSpace(nonce))
            {
                LogHelper.Log(LogLevel.INFO, "nonce is not exists");
                return false;
            }

            if(!await redisClient.KeyExists(SignatureConstants.OAuthClientsTokenSignatureCacheKey))
            {
                await redisClient.HashSet(SignatureConstants.OAuthClientsTokenSignatureCacheKey, "", 0);
                await redisClient.KeyExpire(SignatureConstants.OAuthClientsTokenSignatureCacheKey, DateTime.Now.AddHours(2));
            }

            var signatrueValue = await redisClient.HashGet<int>(SignatureConstants.OAuthClientsTokenSignatureCacheKey, signatureText);
            if (signatrueValue != 0)
            {
                await redisClient.HashIncrement(SignatureConstants.OAuthClientsTokenSignatureCacheKey, signatureText, 1);
                LogHelper.Log(LogLevel.INFO, $"signature is used: {signatureText}");
                return false;
            }

            var dict = new Dictionary<string, string>
            {
                { "method", context.Request.Method.ToLower() },
                { "timestamp", timestamp.ToString() },
                { "client_id",client_id },
                { "nonce",nonce}
            };

            if (context.Request.Query != null && context.Request.Query.Any())
            {
                foreach (var query in context.Request.Query.SkipWhile(s => skipKeys.Contains(s.Key)))
                {
                    dict.Add(query.Key.ToLower(), query.Value);
                }
            }

            if (context.Request.Method.ToLower() == "post" && context.Request.HasFormContentType && context.Request.Form != null && context.Request.Form.Any())
            {
                foreach (var form in context.Request.Form)
                {
                    dict.Add(form.Key, form.Value);
                }
            }

            var signatrue = _handler.Build(dict, secret, signatureText);
            var isok = signatrue.Equals(signatureText, StringComparison.OrdinalIgnoreCase);
            if (isok)
            {
                await redisClient.HashSet(SignatureConstants.OAuthClientsTokenSignatureCacheKey, signatrue, 1);
            }
            else
            {
                LogHelper.Log(LogLevel.INFO, $"signatrue is not same: {signatrue}!={signatureText}");
            }
            return isok;
        }
    }
}
