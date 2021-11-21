using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AntJoin.Core.Utils
{
    /// <summary>
    /// FuncExtensions 类 - 普通方法
    /// 1.    BoPageCount(this int count, int pagesize) 获取总页数
    /// 2.    BoFormatString(this List<int> list) 列表转换成字符串,以 , 连接
    /// 3.    GenerateRandom(int length) 生成随机字母
    /// 4.    BoRegexReplace(this string text, string regex, string replacement) 正则表达式替换
    /// 4.2.  BoRegexReplace(this string text, string regex, string replaceyuan, string replacement) 正则表达式替换
    /// 5.    BoRegexTest(this string text, string regex) 正则表达式匹配
    /// 6.    BoRegexMatches(this string text, string regex) 获取正则表达式匹配内容
    /// 6.2   BoRegexMatch(this string text, string regex) 获取正则表达式匹配内容
    /// 7.    BoClearHtml(this string Htmlstring) 清理所有HTML标记
    /// 8.    BoGetCutString(this string data, string start, string end) 截取字符串
    /// 9.    BoReplaceHtmlTag(this string html, int length = 0) 替换HTMLTag标签
    /// 10.   BoClearHtmlExceptA(this string html) 过滤html标签，汉字间空格，制表符，保留a标签的方法
    /// 11.   BoFileToStream(string fileName) 转换成Stream
    /// 12.   BoStreamToFile(this Stream stream, string fileName) 转换成文件
    /// 13.   BoBytesToString(this byte[] bytes) 串联字节数组, 以 , 串联
    /// 14.   BoBytesToStream(this byte[] bytes) 转换成Stream 
    /// 15.   BoStreamToBytes(this Stream stream) 转换成Bytes
    /// 16.   BoJsonObject<T>(this string data) Json转换成对象
    /// 17.   BoJson(this object obj) 对象转换成Json
    /// 18.   BoArrayListJson(this DataTable dt) DataTable 转 ArrayList 对象
    /// 19.   BoString(this object val) 字符串类型转换
    /// 20.   BoDecimal(this object val) decimal 类型转换(保留两位小数)
    /// 21.   BoDateTime(this object val) 时间类型转换
    /// 22.   BoLong(this object val) int64类型转换
    /// 23.   BoInt32(this object val) int32类型转换
    /// 24.   IsNull(this object obj) 判断对象是否为null
    /// 25.   IsNotNull(this object obj) 判断对象是否不为null
    /// 26.   IsNullOrEmpty(this string str) 判断字符串是否为null或空
    /// 27.   IsNotNullOrEmpty(this string str) 判断字符串是否为,非，null或空
    /// 28.   BoDescriptByEnum(this Enum val) 获取枚举描述
    /// 29.   TrimModelString<T>(this T model) 将model里面的string 类型的属性的前后空格去除
    /// 30.   BoDouble(this object val) double类型转换
    /// 31.   BoTimestamp(this DateTime dt, bool isSecond = true) 时间格式转换成时间戳
    /// 
    /// ----------------------------------------------------------------------------------------------------------------------------
    /// 
    /// MyExtension 类 - 属性
    /// 1.    private static readonly char[] Constant 字符数组,生成随机字母使用 [属性]
    /// 
    /// </summary>
    public static class FuncExtensions
    {
        #region 31. 时间格式转换成时间戳
        /// <summary>
        /// 31.2
        /// 字符串转换成时间戳
        /// </summary>
        /// <param name="dtStr"></param>
        /// <param name="isSecond">是否取秒级时间戳</param>
        /// <returns></returns>
        public static long BoTimestamp(this string dtStr, bool isSecond = true)
        {
            long v;
            DateTime dtStart = new DateTime(1970, 1, 1);
            //秒级 10位，毫秒13秒
            if (isSecond)
            {
                v = (dtStr.BoDateTime().ToUniversalTime().Ticks - dtStart.Ticks) / 10000000;
            }
            else
            {
                v = (dtStr.BoDateTime().Ticks - dtStart.Ticks) / 10000;
            }
            return v;
        }
        /// <summary>
        /// 31
        /// 时间格式转换成时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isSecond"></param>
        /// <returns></returns>
        public static long BoTimestamp(this DateTime dt, bool isSecond = true)
        {
            long v;
            DateTime dtStart = new DateTime(1970, 1, 1);
            //秒级 10位，毫秒13秒
            if (isSecond)
            {
                v = (dt.ToUniversalTime().Ticks - dtStart.Ticks) / 10000000;
            }
            else
            {
                v = (dt.ToUniversalTime().Ticks - dtStart.Ticks) / 10000;
            }
            return v;
        }
        #endregion

        #region 30. double类型转换
        /// <summary>
        /// 30.
        /// 功能: double类型转换
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double BoDouble(this object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToDouble(val);
        }
        #endregion

        #region 29. 将model里面的string 类型的属性的前后空格去除
        /// <summary>
        /// 30. 将model里面的string 类型的属性的前后空格去除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public static void TrimModelString<T>(this T model)
        {
            Type t = model.GetType();
            var properties = t.GetProperties();
            foreach (var info in properties.Where(a => a.PropertyType.FullName == "System.String"))
            {
                string value = (string)info.GetValue(model);
                info.SetValue(model, value?.Trim());
            }
        }
        #endregion


        #region 28. 获取枚举描述
        /// <summary>
        /// 28
        /// 功能: 获取枚举描述
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string BoDescriptByEnum(this Enum val)
        {
            FieldInfo field = val.GetType().GetField(val.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? val.ToString();
        }


        #endregion

        #region 27. 判断字符串是否为，非，null或空
        /// <summary>
        /// 27.
        /// 功能: 判断字符串是否为，非，null或空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        #endregion



        #region 26. 判断字符串是否为null或空
        /// <summary>
        /// 26.
        /// 功能: 判断字符串是否为null或空 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        #endregion


        #region 25. 判断对象是否不为null
        /// <summary>
        /// 25.
        /// 功能: 判断对象是否不为null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }
        #endregion


        #region 24. 判断对象是否为null
        /// <summary>
        /// 24.
        /// 功能: 判断对象是否为null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }
        #endregion


        #region 23. int32类型转换
        /// <summary>
        /// 23
        /// 功能: int32类型转换
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Int32 BoInt32(this object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return 0;
            }
            try
            {
                int result = 0;
                int.TryParse(val.ToString(), out result);
                return result;
                //return Convert.ToInt32(val);
            }
            catch (Exception)
            {

                return 0;
            }
        }
        #endregion


        #region 22. int64类型转换
        /// <summary>
        /// 22.
        /// 功能: int64类型转换
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long BoLong(this object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt64(val);
        }
        #endregion


        #region 21. 时间类型转换
        /// <summary>
        /// 21.
        /// 功能: 时间类型转换
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime BoDateTime(this object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return default(DateTime);
            }
            return Convert.ToDateTime(val);
        }
        #endregion


        #region 20. decimal 类型转换(保留两位小数)
        /// <summary>
        /// 20.
        /// 功能: decimal 类型转换(保留两位小数)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal BoDecimal(this object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return 0.00M;
            }
            try
            {
                decimal result = 0;
                decimal.TryParse(val.ToString(), out result);
                return result;
                //return decimal.Parse(Convert.ToDecimal(val).ToString("0.00"));
            }
            catch (Exception)
            {
                return 0.00M;
            }
        }
        #endregion


        #region 19. 字符串类型转换
        /// <summary>
        /// 19.
        /// 功能: 字符串类型转换
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string BoString(this object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return string.Empty;
            }
            return val.ToString().Trim();
        }
        #endregion
        public static bool BoBoolean(this object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return false;
            }

            var valStr = val.ToString().ToLower();
            if (valStr == "" || valStr == "0" || valStr == "false")
                return false;
            if (valStr == "1" || valStr == "true")
            {
                return true;
            }
            return Convert.ToBoolean(val);
        }

        #region 18. DataTable 转 ArrayList 对象
        /// <summary>
        /// 18.
        /// 功能: DataTable 转 ArrayList 对象 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ArrayList BoArrayListJson(this DataTable dt)
        {
            ArrayList arrayList = new ArrayList();
            foreach (DataRow dataRow in dt.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>(); //实例化一个参数集合
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName]);
                }
                arrayList.Add(dictionary); //ArrayList集合中添加键值
            }
            return arrayList; //返回一个json字符串
        }
        #endregion


        #region 17. 将对象序列号为json字符串
        /// <summary>
        /// 17.
        /// 功能: 将对象序列号为json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string BoJsonString(this object obj, bool lowercase = true)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.Formatting = Formatting.Indented;
            if (lowercase)
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();//首字母小写
            return JsonConvert.SerializeObject(obj, setting);
        }
        #endregion


        #region 16. Json转换成对象
        /// <summary>
        /// 16.
        /// 功能: Json转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T BoJsonObject<T>(this string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        #endregion


        #region 15. 转换成Bytes
        /// <summary>
        /// 15.
        /// 功能: 转换成Bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] BoStreamToBytes(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        #endregion


        #region 14. 转换成Stream
        /// <summary>
        /// 14.
        /// 功能: 转换成Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream BoBytesToStream(this byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        #endregion


        #region 13. 串联字节数组, 以 , 串联
        /// <summary>
        /// 13.
        /// 功能: 串联字节数组, 以 , 串联
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BoBytesToString(this byte[] bytes)
        {
            return string.Join(",", bytes.Select(t => t.ToString()).ToArray());
        }
        #endregion


        #region 12. 转换成文件
        /// <summary>
        /// 12.
        /// 功能: 转换成文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public static void BoStreamToFile(this Stream stream, string fileName)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            // 把 byte[] 写入文件 
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }
        #endregion


        #region 11. 转换成Stream
        /// <summary>
        /// 11.
        /// 功能: 转换成Stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream BoFileToStream(string fileName)
        {
            // 打开文件 
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        #endregion


        #region 10. 过滤html标签，汉字间空格，制表符，保留a标签的方法
        /// <summary>
        /// 10.
        /// 功能: 过滤html标签，汉字间空格，制表符，保留a标签的方法
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string BoClearHtmlExceptA(this string html)
        {
            string acceptable = "a";
            string stringPattern = @"</?(?(?=" + acceptable +
                                   @")notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";
            html = Regex.Replace(html, stringPattern, "");
            html = Regex.Replace(html, @"[\t\n]", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"[\r]", "", RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, @"[\t\n\r\s]","",RegexOptions.IgnoreCase);
            return html;
        }
        #endregion


        #region 9. 替换HTMLTag标签
        /// <summary>
        /// 9.
        /// 功能: 替换HTMLTag标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string BoReplaceHtmlTag(this string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = strText.Replace("&nbsp;", " ");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }
        #endregion


        #region 8. 截取字符串
        /// <summary>
        /// 8.
        /// 功能: 截取字符串
        /// </summary>
        /// <param name="data">内容</param>
        /// <param name="start">开始字符串</param>
        /// <param name="end">结束字符串</param>
        /// <returns></returns>
        public static string BoGetCutString(this string data, string start, string end)
        {
            return Regex.Match(data, "(?<=" + start + ").*?(?=" + end + ")").Value;
        }
        #endregion


        #region 7. 清理所有HTML标记
        /// <summary>
        /// 7.
        /// 功能: 清理所有HTML标记
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string BoClearHtml(this string Htmlstring) //替换HTML标记
        {
            //删除样式
            Htmlstring = Regex.Replace(Htmlstring, @"<style[^>]*?>.*?</style>", "", RegexOptions.IgnoreCase);
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"[\t\n]", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*>;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion


        #region 6.2. 获取正则表达式匹配内容
        /// <summary>
        /// 6.2
        /// 功能: 获取正则表达式匹配内容
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static string BoRegexMatch(this string text, string regex)
        {
            string result = Regex.Match(text, regex, RegexOptions.IgnoreCase).Value;
            return result;
        }
        #endregion


        #region 6. 获取正则表达式匹配内容
        /// <summary>
        /// 6.
        /// 功能: 获取正则表达式匹配内容
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static List<string> BoRegexMatches(this string text, string regex)
        {
            var matches = Regex.Matches(text, regex, RegexOptions.IgnoreCase);
            var result = new List<string>();
            for (int i = 0; i < matches.Count; i++)
            {
                var item = matches[i];
                if (item.Success)
                {
                    result.Add(item.Value);
                }
            }
            return result;
        }
        #endregion


        #region 5. 正则表达式匹配
        /// <summary>
        /// 5.
        /// 功能: 正则表达式匹配
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool BoRegexTest(this string text, string regex)
        {
            return Regex.Match(text, regex, RegexOptions.IgnoreCase).Success;
        }
        #endregion


        #region 4.2. 正则表达式替换
        /// <summary>
        /// 4.2
        /// 功能: 正则表达式替换
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regex"></param>
        /// <param name="replaceyuan"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string BoRegexReplace(this string text, string regex, string replaceyuan, string replacement)
        {
            if (text.BoRegexTest(regex))
            {
                text = text.Replace(replaceyuan, replacement);
            }
            return text;
        }
        #endregion


        #region 4. 正则表达式替换
        /// <summary>
        /// 4.
        /// 功能: 正则表达式替换
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regex"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string BoRegexReplace(this string text, string regex, string replacement)
        {
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            if (r.IsMatch(text))
            {
                text = r.Replace(text, replacement);
            }
            return text;
        }
        #endregion


        #region 3. 生成随机字母
        /// <summary>
        /// 3.
        /// 功能: 生成随机字母
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandom(int length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(52);
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(Constant[rd.Next(52)]);
            }
            return newRandom.ToString();
        }
        #endregion


        #region 2. 列表转换成字符串,以 , 连接
        /// <summary>
        /// 2.
        /// 功能: 列表转换成字符串,以 , 连接
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string BoFormatString(this List<int> list)
        {
            StringBuilder result = new StringBuilder();
            list.ForEach(x =>
            {
                result.Append(string.Format("{0},", x));
            });
            return result.ToString().Substring(0, result.ToString().Length - 1);
        }
        #endregion


        #region 1. 获取总页数
        /// <summary>
        /// 1.
        /// 功能: 获取总页数
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public static int BoPageCount(this int count, int pagesize)
        {
            int pageNum = 0;
            if (count > 0)
            {
                if (count % pagesize > 0)
                {
                    pageNum = count / pagesize + 1;
                }
                else
                {
                    pageNum = count / pagesize;
                }
            }
            return pageNum;
        }
        #endregion



        #region 1. 字符数组,生成随机字母使用 [属性]
        /// <summary>
        /// 1.
        /// 功能: 字符数组,生成随机字母使用 [属性]
        /// </summary>
        private static readonly char[] Constant =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
            'W', 'X', 'Y', 'Z'
        };
        #endregion
    }
}
