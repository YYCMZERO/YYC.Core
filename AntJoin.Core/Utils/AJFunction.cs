using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AntJoin.Core.Utils
{
    public class AJFunction
    {

        //XML序列化、反序列化
        #region 获取序列号
        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <returns></returns>
        public static string GetSerialNo()
        {
            long temp = System.Convert.ToInt64(DateTime.Now.ToString("yMMddHHmmssfff"));
            Random random = new Random();
            int rand = random.Next(0, 9999);
            temp = temp * 10000 + rand;
            return temp.ToString();
        }
        #endregion
        //字符串判断

        #region 是否纯数字
        /// <summary>
        /// 是否纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumberic(string str)
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

        #region 判断是否包含中文
        /// <summary>
        /// 判断是否包含中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsContainChinese(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            string pattern = "^[\u4e00-\u9fa5]+$";
            return Regex.IsMatch(str, pattern);
        }
        #endregion

        #region 是否为合法邮箱
        /// <summary>
        /// 是否为合法邮箱
        /// </summary>
        /// <param name="str">^([a-zA-Z0-9_-])+(\.[a-zA-Z0-9_-]+)*@[A-Za-z0-9-]+((\.)([A-Za-z0-9]+)){1,2}$</param>
        /// <returns></returns>
        public static bool IsEmail(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            string pattern = @"^([a-zA-Z0-9_-])+(\.[a-zA-Z0-9_-]+)*@[A-Za-z0-9-]+((\.)([A-Za-z0-9]+)){1,2}$";
            Regex rex = new Regex(pattern);
            var flag = rex.IsMatch(str);
            return flag;
        }
        #endregion

        #region 验证是否是IP地址
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsIpAddr(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            string pattern = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";
            Regex rex = new Regex(pattern);
            var flag = rex.IsMatch(value);
            return flag;
        }
        #endregion

        #region 过滤xml非法字符串
        /// <summary>
        /// 过滤xml非法字符串
        /// </summary>
        /// <returns>返回过滤后的字符串</returns>
        public static string CheckXmlChar(string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return string.Empty;
            }
            StringBuilder outValueSb = new StringBuilder();
            char current;
            for (int i = 0; i < inputValue.Length; i++)
            {
                current = inputValue[i];
                if ((current == 0x9) || (current == 0xA) || (current == 0xD) || ((current >= 0x20) && (current <= 0xD7FF)) || ((current >= 0xE000) && (current <= 0xFFFD)) || ((current >= 0x10000) && (current <= 0x10FFFF)))
                {
                    outValueSb.Append(current);
                }
            }
            return outValueSb.ToString();
        }
        #endregion

        #region 检测xml是否含有l非法字符串
        /// <summary>
        /// 检测xml是否含有l非法字符串
        /// </summary>
        /// <returns>true：合法，false：不合法</returns>
        public static bool IsRightXmlChar(string inputValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                return true;
            }

            char current;
            for (int i = 0; i < inputValue.Length; i++)
            {
                current = inputValue[i];
                if ((current == 0x9) || (current == 0xA) || (current == 0xD) || ((current >= 0x20) && (current <= 0xD7FF)) || ((current >= 0xE000) && (current <= 0xFFFD)) || ((current >= 0x10000) && (current <= 0x10FFFF)))
                {
                    continue;
                }
                return false;
            }
            return true;
        }
        #endregion


        #region 判断是否手机
        //验证手机号码的主要代码如下：
        public static bool IsPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^[1]+\d{10}");
        }
        #endregion
        //字符串处理

        #region 获取半角字符长度
        /// <summary>
        /// 获取半角字符长度，一个汉字字符将被计算为两个字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetCLength(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }
            return Regex.Replace(input, @"[\u4e00-\u9fa5/g]", "aa").Length;
        }
        #endregion

        #region 移除中文
        /// <summary>
        /// 移除中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveChinese(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            return Regex.Replace(str, @"[\u4e00-\u9fa5]", "");
        }
        #endregion

        #region 获取字符长度
        /// <summary>
        /// 获取字符长度
        /// </summary>
        /// <param name="value">传入字符串</param>
        /// <param name="isByte">中文是否算2个字符,true:中文算2个字符</param>
        /// <returns></returns>
        public static Int32 GetLength(object value, bool isByte = false)
        {
            if (value == null)
            {
                return 0;
            }
            string temp = value.ToString();
            if (string.IsNullOrWhiteSpace(temp))
            {
                return 0;
            }
            if (isByte)
            {
                string pattern = @"[\u4e00-\u9fa5]";
                Regex rex = new Regex(pattern);
                temp = rex.Replace(temp, "aa");
                return temp.Length;
            }
            else
            {
                return temp.Length;
            }

        }
        #endregion


        #region MyRegion
        /// <summary>
        /// 获取随机串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandNumber(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }
        #endregion
    }
}
