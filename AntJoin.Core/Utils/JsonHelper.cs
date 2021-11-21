using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AntJoin.Core.Utils
{
    public class JsonHelper
    {
        private static JsonHelper _instance = null;
        private static readonly object Padlock = new object();

        JsonHelper() { }

        public static JsonHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new JsonHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string SerializeObject(object o)
        {
            return JsonSerializer.Serialize(o);
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// 解析JSON字符串生成动态对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic DeserializeJsonToDynamic(string json)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new DynamicJsonConverter() }
            };
            return JsonSerializer.Deserialize<dynamic>(json, serializerOptions);
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<List<T>>(json);
        }
    }
}
