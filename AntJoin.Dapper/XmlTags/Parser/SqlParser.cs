using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace AntJoin.Dapper.XmlTags.Parser
{
    internal static class SqlParser
    {
        static GenericTokenParser tokenParser=new GenericTokenParser("{","}");
        static readonly string TEXT_PARAM_PLACEHOLDER=":";

        static Regex m_CriteriaRegex;//匹配col1 <> @p1 => [<> @p1]部分
        static Regex CriteriaRegex{
            get{
                if(m_CriteriaRegex==null)
                    m_CriteriaRegex=new Regex("(\\w*\\s|\\W*)"+StatementParser.PREFIX+"\\w*");
                return m_CriteriaRegex;
            }
        }
        public static string parseNullParamInSql(string oldSql,IList<string> bindParams,IDictionary<string,Object> param){
            //if(oldSql.IndexOf(" set ", StringComparison.InvariantCultureIgnoreCase)>0)
            if(oldSql.IndexOf(" set ", StringComparison.CurrentCultureIgnoreCase) >0)
                return parseNullInUpdateSql(oldSql,bindParams,param);

            foreach(string name in bindParams){
                if(!param.ContainsKey(name)|| null ==param[name]){
                    param[name]=null;
                    //\w*\s => 匹配 [like |in ], \W*匹配[>|<|<>| <= ]等
                    oldSql = CriteriaRegex.Replace(oldSql," is null");
                }
            }
            return oldSql;
        }

        //对update语句进行分段处理is null
        static string parseNullInUpdateSql(string oldSql,IList<string> bindParams,IDictionary<string,Object> param){
            string setters,wheres;
            //int i=oldSql.IndexOf(" where ", StringComparison.InvariantCultureIgnoreCase);
            int i = oldSql.IndexOf(" where ", StringComparison.CurrentCultureIgnoreCase);
            if (i>0){
                setters=oldSql.Substring(0,i);
                wheres=oldSql.Substring(i);
            }else{
                setters=oldSql;
                wheres=string.Empty;
            }

            foreach(string name in bindParams){
                if(!param.ContainsKey(name)|| null ==param[name]){
                    param[name]=null;
                    if(setters.IndexOf(StatementParser.PREFIX+name)>0){//设置set为null值
                        param[name]=DBNull.Value;
                    }
                    //\w*\s => 匹配 [like |in ], \W*匹配[>|<|<>| <= ]等
                    wheres = CriteriaRegex.Replace(wheres," is null");//设置where为is null条件
                }
            }
            return setters+wheres;
        }
        
        /**
        * 识别sql条件中用到的每一个参数， 分别放在绑定参数bindParams和格式化参数textParams中
        */
        public static string parseParamsInSql(string oldSql,IList<string> bindParams,IDictionary<string,string> textParams)
        {
            string bindSql = tokenParser.parse("#",oldSql.Trim(), content => {
                        bindParams.Add(content);
                        return "@"+content;
                });

            bindSql = tokenParser.parse("$",bindSql,content=> {
                        // \W 匹配非正常字符
                        String paramName = Regex.Replace(content, "\\W", "");//从格式化串提取参数 "'name%'"==> name
                        textParams[paramName] = content;
                    return TEXT_PARAM_PLACEHOLDER + paramName;
                });//更新为解析后的语句
            return bindSql;
        }

        
        /**
        * 格式化“字符替换参数”，将占位符替换为最终
        */
        public static string parseTextParamsInSql(string oldSql,IDictionary<string,string> textParams,IDictionary<string,object> param){
            String criterion = oldSql;

            if(textParams!=null){
                foreach(String key in textParams.Keys){
                    if(param.ContainsKey(key)){
                        String val=param[key]==null?//如果强制启用了语句，则null参数同于空值
                            string.Empty:param[key].ToString();

                        if(!checkSqlInjectRegex.IsMatch(val))
                            throw new ArgumentException("字符参数'"+key+"'仅允许逗号和文本,当前值检测到异常。 "+val);
                            
                        criterion = getFormattedCriterion(criterion, key,
                                textParams[key],val);
                    }
                }
            }
            return criterion;
        }
        static Regex checkSqlInjectRegex = new Regex("[,\\w\\d|\\u4e00-\\u9fa5]+");
        /**
        * ${param}时， 是否存在格式化字符如： ${'param'}则单引号就是一种格式处理
        * 格式化参数值：format='name%' , pvs=a,b,c ==> 'a%','b%','c%'
        */
        static string getFormattedCriterion(string criterion,string paramName,string format,string vals){
            if(format.Equals(paramName)) 
                return criterion.Replace(TEXT_PARAM_PLACEHOLDER + paramName,vals);
            
            
            StringBuilder sb = new StringBuilder();
            
            string[] pvs = vals.Split(',');
            if(pvs.Length>1){
                //多值like特殊处理
                if(criterion.ToLower().Contains(" like ")){
                    var reg = new Regex(@"(\s){0,1}(and)+(\s)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
                    criterion = reg.Replace(criterion, "");

                    foreach (String pv in pvs){//format='name%' , pvs=a,b,c ==> 'a%','b%','c%'
                        sb.Append(
                            criterion.Replace(TEXT_PARAM_PLACEHOLDER+paramName,
                                    format.Replace(paramName, pv)) );
                        sb.Append(" or ");
                    }
                    sb.Remove(sb.Length-4,4);//trim ending 'OR'
                    return "and ("+sb.ToString()+")";//like 直接返回
                } 
            }

            //无论多值、单值，都需要进行字符串重新格式化
            foreach(String pv in pvs){//in (':param') , pvs=a,b,c ==> in ('a','b','c')
                sb.Append(format.Replace(paramName, pv));
                sb.Append(',');
            }
            sb.Remove(sb.Length-1,1);//去除结尾的逗号
            
            return criterion.Replace(TEXT_PARAM_PLACEHOLDER + paramName,  sb.ToString());
        }

    }
}