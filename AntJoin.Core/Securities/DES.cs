using AntJoin.Core.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntJoin.Core.Security
{
    /// <summary>
    /// DES 对称加密
    /// </summary>
    public class Des
    {
        private static readonly IBlockCipher engine = new DesEngine();
        private const string IV = "&xp2o@bN";



        private Des()
        {

        }
       
        /// <summary>
        /// 使用DES加密
        /// </summary>
        /// <param name="plainText">需要加密的字符串</param>
        /// <param name="keys">加密字符串的密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string plainText, string keys)
        {
            byte[] ptBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] rv = DoEncrypt(keys, ptBytes);
            StringBuilder ret = new StringBuilder();
            foreach (byte b in rv)
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }


        private static byte[] DoEncrypt(string keys, byte[] ptBytes)
        {
            var key = Encoding.UTF8.GetBytes(keys);
            var iv = Encoding.UTF8.GetBytes(IV);
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());
            cipher.Init(true, new ParametersWithIV(new DesParameters(key), iv));
            byte[] rv = new byte[cipher.GetOutputSize(ptBytes.Length)];
            int tam = cipher.ProcessBytes(ptBytes, 0, ptBytes.Length, rv, 0);

            cipher.DoFinal(rv, tam);
            return rv;
        }



        /// <summary>
        /// 使用DES解密
        /// </summary>
        /// <param name="cipherText">需要加密的字符串</param>
        /// <param name="keys">加密字符串的密钥</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string cipherText, string keys)
        {
            byte[] inputByteArray = new byte[cipherText.Length / 2];
            for (int x = 0; x < cipherText.Length / 2; x++)
            {
                int i = (Convert.ToInt32(cipherText.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            var rv = DoDecrypt(keys, inputByteArray);
            List<byte> lb = new List<byte>();
            foreach (var b in rv)
            {
                if (b != '\0')
                {
                    lb.Add(b);
                }
            }

            var str = Encoding.UTF8.GetString(lb.ToArray());
            return str;
        }


        private static byte[] DoDecrypt(string keys, byte[] cipherText)
        {
            var key = Encoding.UTF8.GetBytes(keys);
            var iv = Encoding.UTF8.GetBytes(IV);
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
            cipher.Init(false, new ParametersWithIV(new DesParameters(key), iv));
            byte[] rv = new byte[cipher.GetOutputSize(cipherText.Length)];
            int tam = cipher.ProcessBytes(cipherText, 0, cipherText.Length, rv, 0);

            cipher.DoFinal(rv, tam);

            return rv;
        }
    }
}
