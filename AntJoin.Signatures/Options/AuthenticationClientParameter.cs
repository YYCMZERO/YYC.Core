using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

namespace AntJoin.Signatures
{
    /// <summary>
    /// 客户端授权参数
    /// </summary>
    public class AuthenticationClientParameter
    {
        /// <summary>
        /// 启动签名
        /// </summary>
        public bool EnabledSignature { get; set; } = true;

        /// <summary>
        /// 是否启用同设备端设备互踢功能
        /// </summary>
        public bool EnableEquipmentKicks { get; set; } = true;

        /// <summary>
        /// 缓存连接
        /// </summary>
        public CacheConnectionParameter CacheConnection { get; set; } = new CacheConnectionParameter();

        /// <summary>
        /// JWT配置
        /// </summary>
        public Action<JwtBearerOptions> JwtOptions { get; set; }
    }
}
