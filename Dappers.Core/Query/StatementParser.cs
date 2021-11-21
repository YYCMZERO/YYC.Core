using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using System.IO;
using Dappers.XmlTags;
using System.Xml.Serialization;

namespace Dappers
{
    public class StatementParser{
        public readonly static string PREFIX="@";
        public static void ParseDynamicSql(QueryInfo info)
        {
            info.CustomSQL =      ParseDynamicSql(info.CustomSQL, info.Parameters, info.Wheres);
        }
        public static string ParseDynamicSql(string mapSql, IDictionary<string, object> param)
        {
            return ParseDynamicSql(mapSql, param, null);
        }
        private static string ParseDynamicSql(string mapSql, IDictionary<string, object> p, IDictionary<string, string> where)
        {
            SqlItem info= StatementParser.GetMappedStatement(mapSql);
            if(info==null) return mapSql;

            return info.dynamic(p);//会修改参数p的列表
        }
        
        
        public static string GetMappedStaticSql(string sqlName)
        {
            SqlItem item = GetMappedStatement(sqlName);
            if(item==null) return sqlName;

            return item.dynamic(null);
        }
        /// <summary>
        /// no mapping? return original. load config in "DB_ADAPTERS".
        /// </summary>
        public static SqlItem GetMappedStatement(string sqlName)
        {
            if (sqlName == null || sqlName.IndexOf(" ")>0)
                return null;

            //if(!StatementCache.ContainsKey(sqlName))
            //{
            //    if(StatementCache.Count==0)
            //        ReadMappedXmlFiles();
            //    //if(!StatementCache.ContainsKey(sqlName))
            //    //    throw new ArgumentOutOfRangeException("无效的Sql配置项："+sqlName);
            //}
            return (SqlItem)StatementCache.Get(sqlName);
        }
        //static void ReadMappedXmlFiles(){
        //    //string config = Dappers.Cfg.ConfigManager.GetRootConfig("DB_ADAPTERS");
        //    string path = config.Substring(0,config.LastIndexOf("\\"));
        //    string file = config.Substring(config.LastIndexOf("\\")+1);
        //    if(path.IndexOf(":")<0){
        //        path=Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,path);
        //    }

        //     string[] files = Directory.GetFiles(path, file, SearchOption.TopDirectoryOnly);
        //     ReadXmlToStatements(files);
        //}
        static void ReadXmlToStatements(string[] files){
            XmlSerializer xs = new XmlSerializer(typeof(SqlRoot));

            foreach(string file in files)
            {
                using(FileStream fileRead=new FileStream(file,FileMode.Open,FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fileRead,System.Text.Encoding.GetEncoding("utf-8")))
                    {
                        try
                        {
                           SqlRoot root = (SqlRoot)xs.Deserialize(sr);
                           foreach(var kp in root.GetItems())
                                cacheStatements.Insert(kp.Key,kp.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("解析Sql配置文件错误："+file, ex);
                        }
                    }
                }
            }
        }

        static Dappers.Context.ICache cacheStatements = new Dappers.Context.Impl.LruCache(null);
        public static Dappers.Context.ICache StatementCache{
            get{
                return cacheStatements;
            }
        }
        static object syncObject = new object();
       
    }
}