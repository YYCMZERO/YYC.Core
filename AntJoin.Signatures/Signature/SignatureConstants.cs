namespace AntJoin.Signatures
{
    public class SignatureConstants
    {
        /// <summary>
        /// 签名的Redis客户端名称
        /// </summary>
        public const string RedisClientName = "Default";


        /// <summary>
        /// 键的前缀
        /// </summary>
        public const string DbKeyPrefix = "Signatures";


        /// <summary>
        /// 签名规则名称
        /// </summary>
        public const string PolicyName = "Signature";


        /// <summary>
        /// 认证客户端Token签名缓存
        /// </summary>
        public const string OAuthClientsTokenSignatureCacheKey = "OAuth_ClientsTokenSignature";


        /// <summary>
        /// 认证客户端Secret缓存
        /// </summary>
        public const string OAuthClientsSecretCacheKey = "OAuth_ClientsSecret";
    }
}
