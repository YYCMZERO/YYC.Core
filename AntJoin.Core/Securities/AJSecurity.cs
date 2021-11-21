using System;
using System.Text;

namespace AntJoin.Core.Security
{
    /// <summary>
    /// AntJoin 内部加解密
    /// </summary>
    public class AJSecurity
    {
        private const string defaultKey = "QCLOAe7##&&K";

        private AJSecurity()
        {

        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            return Des.Encrypt(plainText, Md5.Hash(defaultKey));
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Decrypt(string plainText)
        {
            return Des.Decrypt(plainText, Md5.Hash(defaultKey));
        }



        #region 自定义对称性加密
        /// <summary>
        /// 对称性加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string AJEncrypt(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return source;
            }
            byte[] srcByte = Encoding.ASCII.GetBytes(source);
            int srclen = string.IsNullOrEmpty(source) ? 0 : srcByte.Length;

            int inlen = (1 + srclen + 2) + 1;
            int outlen = (3 + srclen) / 3 * 4 + 1;
            char[] buf = new char[inlen + outlen];
            char[] inbuf = buf;
            char[] outbuf = new char[inlen + outlen];
            for (int i = 0; i < inlen + outlen; i++)
            {
                buf[i] = '0';
            }

            // 复制源串到(inbuf+1)，每个字符进行移位变换 
            for (int i = 0; i < srclen; i++)
            {
                inbuf[i + 1] = EncryptChar((char)srcByte[i]);
            }
            // 设置长度标记字符 inbuf[0] 
            // 移位变换可能产生0字符，用最低两位代表inbuf的长度, 00:3n, 01:3n+1,10:3n+2 
            inbuf[0] = (char)(randchar() & (~0x03));    // 最低两位为0时，leadlen % 3 == 0 

            int actlen = srclen + 1;
            if (actlen % 3 == 1)        // 原长3n+1，补两个随机字符保证inbuf长度为3n 
            {
                inbuf[0] |= (char)(0x01);
                inbuf[actlen] = randchar();
                inbuf[actlen + 1] = randchar();
                actlen += 2;
            }
            else if (actlen % 3 == 2)    // 原长3n+2，补1个随机字符保证inbuf长度为3n 
            {
                inbuf[0] |= (char)0x02;
                inbuf[actlen] = randchar();
                actlen++;
            }

            // 从inbuf转换出outbuf，outbuf由字母、数字、'_'、'.'组成，长度为4的倍数 
            ToBase64(inbuf, actlen, outbuf);
            string strResult = new string(outbuf);
            strResult = strResult.Replace("\0", "");

            return strResult;
        }

        /// <summary>
        /// 字符串转换为Base64
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="len"></param>
        /// <param name="outstr"></param>
        private static void ToBase64(char[] instr, int len, char[] outstr)
        {
            int i, j;
            char ch1, ch2, ch3;
            i = 0;
            j = 0;
            while (i + 2 < len)
            {
                ch1 = instr[i];
                ch2 = instr[i + 1];
                ch3 = instr[i + 2];
                outstr[j] = UnIndex64(ch1 >> 2);
                outstr[j + 1] = UnIndex64(((ch1 & 0x3) << 4) | (ch2 >> 4));
                outstr[j + 2] = UnIndex64(((ch2 & 0x0f) << 2) | (ch3 >> 6));
                outstr[j + 3] = UnIndex64(ch3 & 0x3f);
                i += 3;
                j += 4;
            }
            outstr[j] = '\0';
        }

        /// <summary>
        /// 根据数值得到相应字符
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        private static char UnIndex64(int nIndex)
        {
            char ch;
            nIndex %= 64;

            if (nIndex < 26)
            {
                ch = (char)('A' + nIndex);
            }
            else if (nIndex < 52)
            {
                ch = (char)('a' + nIndex - 26);
            }
            else if (nIndex < 62)
            {
                ch = (char)('0' + nIndex - 52);
            }
            else if (nIndex == 62)
            {
                ch = '_';
            }
            else if (nIndex == 63)
            {
                ch = '.';
            }
            else
            {
                ch = 'A';
            }
            return ch;
        }
        #endregion

        #region 自定义对称性解密
        /// <summary>
        /// 对称性解密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string AJDecrypt(string source)
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
        static char randchar()
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
        private static void UnBase64(char[] instr, int len, char[] outstr)
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
        private static char EncryptChar(char c)
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
        private static int Index64(char ch)
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
