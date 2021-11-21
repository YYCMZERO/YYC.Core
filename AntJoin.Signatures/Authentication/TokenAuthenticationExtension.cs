using AntJoin.Signatures.Authentication.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace AntJoin.Signatures
{
    public static class TokenAuthenticationExtension
    {
        /// <summary>
        /// 添加Token验证
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtTokenAuthentication(this AuthenticationBuilder builder, Action<JwtBearerOptions> configureOptions)
        {
            return builder.AddJwtTokenAuthentication<JwtBearerTokenValidateHandler>(configureOptions);
        }



        /// <summary>
        /// 添加Token验证
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtTokenAuthentication<THandler>(this AuthenticationBuilder builder, Action<JwtBearerOptions> configureOptions)
            where THandler : JwtBearerHandler
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions>());
            return builder.AddScheme<JwtBearerOptions, THandler>(JwtBearerDefaults.AuthenticationScheme, null, configureOptions);
        }
    }
}
