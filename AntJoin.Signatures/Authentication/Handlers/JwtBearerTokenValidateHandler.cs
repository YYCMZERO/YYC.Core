using AntJoin.Log;
using AntJoin.Redis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AntJoin.Signatures.Authentication.Handlers
{
    public class JwtBearerTokenValidateHandler : JwtBearerHandler
    {
        private readonly IRedisClientProvider _redisClientProvider;
        private bool _isTokenCover = false;

        public JwtBearerTokenValidateHandler(IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IRedisClientProvider redisClientProvider,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _redisClientProvider = redisClientProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headerHasToken = Request.Headers.TryGetValue(HeaderNames.Authorization, out var token) &&
                                 !string.IsNullOrWhiteSpace(token.ToString()) &&
                                 !token.ToString().Trim().Equals(JwtBearerDefaults.AuthenticationScheme);

            if (!headerHasToken)
            {
                if (Request.Query.TryGetValue("token", out token))
                {
                    Request.Headers.TryAdd(HeaderNames.Authorization, $"Bearer {token}");
                }
            }

            var result = await base.HandleAuthenticateAsync();
            if (result.Succeeded)
            {
                var redisClient = _redisClientProvider.Get(SignatureConstants.RedisClientName);
                var cid = result.Principal?.Claims.FirstOrDefault(s => s.Type == "client_id")?.Value ?? "";
                var sub = result.Principal?.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
                
                var key = $"{cid}:{sub}_token";
                var vToken = result.Properties?.Items.FirstOrDefault(s => s.Key == ".Token.access_token").Value ?? "";
                var eToken = await redisClient.StringGet<string>(key);

                if (string.IsNullOrEmpty(eToken))
                {
                    LogHelper.Log(Log.LogLevel.WARN, $"用户{sub}已登出");
                    return AuthenticateResult.Fail("user logout and token invalid");
                }

                if (eToken != vToken)
                {
                    _isTokenCover = true;
                    LogHelper.Log(Log.LogLevel.WARN, $"用户{sub}在新设备上登录，此设备被迫下线，token信息被更新，无法使用原有token访问");
                    return AuthenticateResult.Fail("token map be update or user login on other devices");
                }
            }
            return result;
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            await base.HandleChallengeAsync(properties);
            if (_isTokenCover)
            {
                base.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            }
        }

    }
}
