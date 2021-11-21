using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Xml.Serialization;
using System.Text;
using AntJoin.Dapper.XmlTags.Parser;

namespace AntJoin.Dapper.XmlTags
{
    [XmlRoot("sql")]
    public class SqlItem
    {
        //protected static Logger log = LogManager.GetCurrentClassLogger();

        //数据源，从sqlRoot复制下来
        public string source;
            
        [XmlAttribute("id")]
        public string id;

        [XmlText]
        public string _cData;
        [XmlAttribute("value")]
        public string _valueAttr;
        
        //经过首次init 调用parseParamsInSql之后更新，
        public string value{
            get{
                return _cData??_valueAttr;
            }
        }
        
        [XmlElement("if")]
        public SqlIf[] sqlIfs;
        
        [XmlElement("bind")]
        public List<SqlBind> sqlBinds;

        void parseBindsParam(IDictionary<string,object> param){
            foreach(SqlBind bds in this.sqlBinds) {
                param[bds.name] = bds.evalValue(param);//用计算的结果替换参数值
            }
        }
        List<string> bindParams;
        /**
        * 参数需替换为文本
        */
        IDictionary<string,string> textParams;
        void parseParamsInSql(){
            bindParams = new List<string>();
            textParams = new Dictionary<string,string>();
            this._cData = SqlParser.parseParamsInSql(this.value, bindParams, textParams);

            if(textParams.Count==0) textParams=null;
            if(sqlBinds!=null && sqlBinds.Count==0) sqlBinds=null;
            if(sqlIfs==null) sqlIfs=new SqlIf[0];//empty Array
        }

        public string dynamic(IDictionary<string,object> param,List<KeyValuePair<string,object>> usedParamsInSql)
        {
            if(sqlBinds!=null)//更新参数值
                parseBindsParam(param);
            
            if(bindParams==null)//是否判断解析了主Sql语句
                this.parseParamsInSql();
            
            string sql = this.value;//SqlParser.parseNullParamInSql(this.value,this.bindParams,param);
            StringBuilder sb = new StringBuilder(this.textParams==null?sql://Sql主语句是否需要替换格式参数
                SqlParser.parseTextParamsInSql(sql,this.textParams, param) );
            this.setParams(param, usedParamsInSql);
            
            foreach(SqlIf ifs in this.sqlIfs) {
                if(ifs.testAssert(param)){//01. 执行test检查
                    sb.Append("\r\n ");
                    sb.Append(ifs.getFormattedTextParamSql(param));//02. 获得格式化后最终语句
                    ifs.setParams(param, usedParamsInSql);//03. 复制出当前if绑定用的参数
                }
            }
            return sb.ToString();
        }

        public string dynamic(IDictionary<string,object> param)
        {
            if(param==null)//静态sql
                return this.value;

            List<KeyValuePair<string,object>> usedParamsInSql=new List<KeyValuePair<string,object>>();
            string sql = dynamic(param,usedParamsInSql);

            #region 删除当前语句中并未使用的参数
            if(param.Count>0){
                List<string> paramsToRemove=new List<string>();
                foreach (KeyValuePair<string,object> kp in usedParamsInSql)
                {
                    if(!param.ContainsKey(kp.Key))
                        paramsToRemove.Add(kp.Key);
                }
                foreach (string key in paramsToRemove)
                {
                    param.Remove(key);
                }
            }
            #endregion

            return sql;
        }
        
        /**
        * 将语句使用到的参数， 按顺序添加到结果Hash中
        * @param param
        * @param usedParamsInSql
        */
        void setParams(IDictionary<string,object> param,List<KeyValuePair<string,object>> usedParamsInSql)
        {
            foreach(string key in this.bindParams){
                    usedParamsInSql.Add(new KeyValuePair<string,object>(key,
                        param.ContainsKey(key)?param[key]:DBNull.Value) );
            }
        }
        
    }
}