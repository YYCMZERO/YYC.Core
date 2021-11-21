using AntJoin.Core.Security;

namespace AntJoin.Core.Common
{
    public class ConnectionStringDecrypt
    {
        /// <summary>
        /// 字符串解密，以后连接字符串整个都需要加密
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <returns></returns>
        public static string Decrypt(string connectionStr)
        {
            if (!string.IsNullOrWhiteSpace(connectionStr))
            {
                connectionStr = AJSecurity.Decrypt(connectionStr);
            }
            return connectionStr;
        }
    }
}
