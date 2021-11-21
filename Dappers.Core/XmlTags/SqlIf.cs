using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using System.Xml.Serialization;
using System.Text;
using Dappers.XmlTags.Parser;

namespace Dappers.XmlTags
{
    [XmlRoot("if")]
    public class SqlIf
    {
        //protected static Logger log = LogManager.GetCurrentClassLogger();

        [XmlAttribute("test")]
        public string test;

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

        List<string> bindParams;

        /**
        * 获取本语句需绑定参数名
        */
        public List<string> getParams()
        {
            return bindParams;
        }

        /**
        * 参数需替换为文本
        */
        IDictionary<string,string> textParams;

        void parseParamsInSql(){
            bindParams = new List<string>();
            textParams = new Dictionary<string,string>();

            this._cData = SqlParser.parseParamsInSql(this.value, bindParams, textParams);
            if(textParams.Count==0) textParams=null;
        }

        public string getFormattedTextParamSql(IDictionary<string,Object> param){
            string sqlWhere=this.value;
            //if(!string.IsNullOrEmpty(this.test))
            //    sqlWhere = SqlParser.parseNullParamInSql(sqlWhere,this.bindParams,param);
            
            return SqlParser.parseTextParamsInSql(sqlWhere,this.textParams, param);
        }
        
        /**
        * 当前参数集是否可以启用本if语句？
        */
        public bool testAssert(IDictionary<string,Object> param){
            if(bindParams==null)
                this.parseParamsInSql();//criterion初始化入口！！！！
            
            if(this.test==null || this.test.Length==0){//未定义test条件
                    List<string> bp = this.getParams();
                    if(bp.Count==0){//无绑定参数， 检测格式参数
                        if(textParams==null)//无任何参数要求，直接添加
                            return true;
                        else{
                            string p=textParams.Keys.First();
                            if(param.ContainsKey(p) && //文本参数必须存在、不为空
                                    null != param[p] &&
                                    !"".Equals(param[p]) )
                                return true;
                        }
                    }
                    else{
                        string p2 = bp[0];
                        if(param.ContainsKey(p2) && //任意参数存在、不为空
                                null != param[p2] &&
                                !"".Equals(param[p2]) )
                            return true;
                    }
            }
            //else
            //    return (bool)ExpressionEvaluator.GetValue(null,this.test, param);
            
            return false;
        }
        
        /**
        * 将语句使用到的参数， 按顺序添加到结果Hash中
        * @param param
        * @param usedParamsInSql
        */
        public void setParams(IDictionary<string,Object> param,List<KeyValuePair<string,object>> usedParamsInSql)
        {
            foreach(string key in this.bindParams){
                if(param.ContainsKey(key))
                    usedParamsInSql.Add(new KeyValuePair<string,object>(key,
                        param.ContainsKey(key)?param[key]:DBNull.Value) );
            }
        }        
    }
}