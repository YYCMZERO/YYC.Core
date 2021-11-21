using System;
using System.Collections.Generic;
using System.IO;
using AntJoin.Dapper.XmlTags;
using System.Xml.Serialization;

namespace AntJoin.Dapper
{
    public class StatementParser
    {
        public readonly static string PREFIX = "@";
        public static void ParseDynamicSql(QueryInfo info)
        {
            info.CustomSQL = ParseDynamicSql(info.CustomSQL, info.Parameters, info.Wheres);
        }
        public static string ParseDynamicSql(string mapSql, IDictionary<string, object> param)
        {
            return ParseDynamicSql(mapSql, param, null);
        }
        private static string ParseDynamicSql(string mapSql, IDictionary<string, object> p, IDictionary<string, string> where)
        {
            SqlItem info = StatementParser.GetMappedStatement(mapSql);
            if (info == null) return mapSql;

            return info.dynamic(p);//会修改参数p的列表
        }


        public static string GetMappedStaticSql(string sqlName)
        {
            SqlItem item = GetMappedStatement(sqlName);
            if (item == null) return sqlName;

            return item.dynamic(null);
        }


        /// <summary>
        /// no mapping? return original. load config in "DB_ADAPTERS".
        /// </summary>
        public static SqlItem GetMappedStatement(string sqlName)
        {
            if (sqlName == null || sqlName.IndexOf(" ") > 0)
            {
                return null;
            }
            return (SqlItem)StatementCache.Get(sqlName);
        }

        static void ReadXmlToStatements(string[] files)
        {
            XmlSerializer xs = new XmlSerializer(typeof(SqlRoot));

            foreach (string file in files)
            {
                using (FileStream fileRead = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fileRead, System.Text.Encoding.GetEncoding("utf-8")))
                    {
                        try
                        {
                            var root = (SqlRoot)xs.Deserialize(sr);
                            foreach (var kp in root.GetItems())
                            {
                                StatementCache.Insert(kp.Key, kp.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("解析Sql配置文件错误：" + file, ex);
                        }
                    }
                }
            }
        }

        public static AntJoin.Dapper.Context.ICache StatementCache { get; } = new Context.Impl.LruCache(null);
        static object syncObject = new object();

    }
}