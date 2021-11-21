using System;
using System.Security.Cryptography;
using System.Text;

namespace AntJoin.Core.Security
{
    /// <summary>
    /// RSA 非对称加密
    /// </summary>
    public class Rsa
    {
        private Rsa()
        {

        }


        /// <summary>
        /// RSA加密加密
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encrypt(string publicKey, string content, string encoding = "utf-8")
        {
            string encryptedContent = string.Empty;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                byte[] encryptedData = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(content), false);
                encryptedContent = Convert.ToBase64String(encryptedData);
            }
            return encryptedContent;
        }


        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decrypt(string privateKey, string content, string encoding = "utf-8")
        {
            string decryptedContent = string.Empty;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(content), false);
                decryptedContent = Encoding.GetEncoding(encoding).GetString(decryptedData);
            }
            return decryptedContent;
        }
    }
}
