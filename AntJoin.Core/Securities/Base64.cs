using System;
using System.Text;

namespace AntJoin.Core.Security
{
    /// <summary>
    /// Base64加密解密
    /// </summary>
    public class Base64
    {
        private Base64()
        {

        }


        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="codeName">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string Encode(string source, string encoding = "utf-8")
        {
            byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(source);
            string strEncode;
            try
            {
                strEncode = Convert.ToBase64String(bytes);
            }
            catch
            {
                strEncode = source;
            }
            return strEncode;
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Decode(string result, string encoding = "utf-8")
        {
            string decode;
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = Encoding.GetEncoding(encoding).GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
    }
}
