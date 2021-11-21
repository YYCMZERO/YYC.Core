using IdentityServer4.Configuration;
using System;

namespace AntJoin.Signatures
{
    /// <summary>
    /// 服务器端授权参数
    /// </summary>
    public class AuthenticationServiceParameter
    {
        /// <summary>
        /// 是否启用同设备端设备互踢功能
        /// </summary>
        public bool EnableEquipmentKicks { get; set; } = true;

        /// <summary>
        /// IdentityServer4配置
        /// </summary>
        public Action<IdentityServerOptions> IdentityServerOption { get; set; }

        /// <summary>
        /// 缓存连接，必须配置
        /// </summary>
        public CacheConnectionParameter CacheConnection { get; set; } = new CacheConnectionParameter();
    }
}
