using AntJoin.Core.Domains;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AntJoin.Core.Extensions
{
    /// <summary>
    /// HttpContext扩展
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static LoginUser GetLoginUser(this HttpContext context)
        {
            var userData = context.User.FindFirst(AntjoinClaimTypes.UserInfo)?.Value;
            return string.IsNullOrWhiteSpace(userData) ? new LoginUser() : JsonConvert.DeserializeObject<LoginUser>(userData);
        }
    }
}
