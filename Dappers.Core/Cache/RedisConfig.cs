using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Dappers.Cache
{
    class RedisConfig
    {
        #region Host
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }
        #endregion

        #region Port
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }
        #endregion

        #region Password
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        #endregion
        public int DbName { get; set; }

        private static IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        private static IConfigurationRoot configuration = builder.Build();

        public RedisConfig(string configName = "RedisConnectionStrings:")
        {
            int portDefault = 6379;
            string hostDefault = "127.0.0.1";
            int defaultDb = 0;//选择数据库  默认值为0
            string redisIp = configuration[configName + "RedisIp"];
            string redisPort = configuration[configName + "RedisPort"];
            string redisPassword = configuration[configName + "RedisPassword"];
            string redisDb = configuration[configName + "RedisDb"];
            //获取IP
            if (!string.IsNullOrWhiteSpace(redisIp))
            {
                Host = redisIp;
            }
            else
            {
                Host = hostDefault;
            }
            //获取端口号
            if (IsNumberic(redisPort))
            {
                Port = Convert.ToInt32(redisPort);
            }
            else
            {
                Port = portDefault;
            }
            if (!string.IsNullOrWhiteSpace(redisDb))
            {
                DbName = Convert.ToInt32(redisDb);
            }
            else
            {
                DbName = defaultDb;
            }
            //密码
            if (!string.IsNullOrWhiteSpace(redisPassword))
            {
                if (string.Equals(redisPassword, "southinfo@2012"))
                {
                    Password = redisPassword;
                }
                else
                {
                    Password = SIDecrypt(redisPassword);
                }
            }
            else
            {
                Password = "southinfo@2012";
            }
        }

        #region 是否纯数字
        /// <summary>
        /// 是否纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsNumberic(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Regex rex = new Regex(pattern);
            return rex.IsMatch(str);
        }
        #endregion

        #region 自定义对称性解密
        /// <summary>
        /// 对称性解密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string SIDecrypt(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return source;
            }
            int srclen = string.IsNullOrEmpty(source) ? 0 : (int)(source.Length / 4 * 4);
            int len = srclen * 3 / 4;
            if (0 == len)
            {
                return "";
            }
            char[] chBuf = new char[len + 1];
            UnBase64(source.ToCharArray(), srclen, chBuf);
            if (1 == (chBuf[0] & 0x03))
                len -= 2;
            else if (2 == (chBuf[0] & 0x03))
                len--;
            chBuf[len] = (char)0x00;
            byte[] resultBytes = new byte[len - 1];
            for (int i = 1; i < len; i++)
            {
                resultBytes[i - 1] = (byte)EncryptChar(chBuf[i]);
            }
            string strResult = Encoding.ASCII.GetString(resultBytes);

            return strResult;
        }

        /// <summary>
        /// 随机字符
        /// </summary>
        /// <returns></returns>
        char randchar()
        {
            Random ran = new Random(unchecked((int)DateTime.Now.Ticks));
            char c = (char)ran.Next(65, 90);
            return 0 == c ? '\xAA' : c;
        }

        /// <summary>
        /// 解码Base64编码的字符串
        /// </summary>
        /// <param name="instr">instr由字符(字母、数字、'_'、'.')组成的字符串，长度为len，为4的倍数</param>
        /// <param name="len">len instr的字符个数，为3的倍数 </param>
        /// <param name="outstr">填充由字符(字母、数字、'_'、'.')组成的字符串，长度为4的倍数 </param>
        /// <remarks> An indexed by characters (string conversion letters, Numbers, '_', '. ') composition string</remarks>
        private void UnBase64(char[] instr, int len, char[] outstr)
        {
            int i, j;
            int ch1, ch2, ch3, ch4;
            i = 0;
            j = 0;
            while (i + 3 < len)
            {
                ch1 = Index64(instr[i]);
                ch2 = Index64(instr[i + 1]);
                ch3 = Index64(instr[i + 2]);
                ch4 = Index64(instr[i + 3]);
                outstr[j] = (char)((ch1 << 2) | ((ch2 >> 4) & 0x3));
                outstr[j + 1] = (char)((ch2 << 4) | ((ch3 >> 2) & 0xf));
                outstr[j + 2] = (char)((ch3 << 6) | ch4);
                i += 4;
                j += 3;
            }
            outstr[j] = '\0';
        }

        /// <summary>
        ///  EncryptChar 对一个字符进行移位变换：最高位不变，其余7位对调（1-7,2-6,3-5）
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private char EncryptChar(char c)
        {
            char x = (char)0x00;

            x += (char)(c & 0x80);
            x += (char)((c & 0x40) >> 6);
            x += (char)((c & 0x20) >> 4);
            x += (char)((c & 0x10) >> 2);
            x += (char)(c & 0x08);
            x += (char)((c & 0x04) << 2);
            x += (char)((c & 0x02) << 4);
            x += (char)((c & 0x01) << 6);

            return x;
        }

        /// <summary>
        /// 字符转换为相应的数值
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private int Index64(char ch)
        {
            int nIndex;
            int n;

            if (ch >= 'A' && ch <= 'Z')
            {
                nIndex = ch - 'A';
            }
            else if (ch >= 'a' && ch <= 'z')
            {
                nIndex = 26 + ch - 'a';
            }
            else if (int.TryParse(ch.ToString(), out n))
            {
                nIndex = 52 + ch - '0';
            }
            else if (ch == '_')
            {
                nIndex = 62;
            }
            else if (ch == '.')
            {
                nIndex = 63;
            }
            else
            {
                nIndex = 0;
            }

            return nIndex;
        }
        #endregion
    }
}
