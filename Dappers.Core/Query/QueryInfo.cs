using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Linq;

namespace Dappers
{
    public delegate QueryInfo FindByQueryInfo(QueryInfo info);
    [DataContract(Namespace="Dappers")]
    public class QueryInfo : ICloneable
    {
        protected string SelectFields { get; set; }
        /// <summary>
        /// NamedQuery/SP/SQL时:QueryObject/CustomSQL
        /// </summary>
        public QueryInfo()
        {
        }

        public QueryInfo(Type mappingType)
        {
            this.MappingType = mappingType;
        }
        /// <summary>
        /// 设置为1 时，查询总数并分页， 设置为-1时，仅按要求分页，不查询总数
        /// </summary>
        public QueryInfo DoCount(int i)
        {
            this.totalCount = i;
            return this;
        }

        public QueryInfo OrderBy(string orderby)
        {
            if (orderby==null)
                return this;

            if(this.MappingType!=null)
            {
                string[] o=orderby.Split(' ');
                if (o.Length < 3)
                {
                    this.OrderBys.Add(
                        TryGetMappingColumn(o[0],false)
                        +" "+ (o.Length == 2 ? o[1] : string.Empty)
                        );
                    return this;
                }
            }
            this.OrderBys.Add(orderby);
            return this;
        }
        public QueryInfo GroupBy(string groupby)
        {
            this.groupBy = groupby;
            return this;
        }

        private int startRecord;
        public int StartRecord
        {
            get { return startRecord; }
            set { startRecord = value; }
        }
        private int pageSize;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }
        private int totalCount;
        [DataMember]
        /// <summary>
        /// -1：仅获取指定数据； 1：获取总条数，并取得指定数据
        /// </summary>
        public int TotalCount
        {
            get { return totalCount; }
            set { totalCount=value; }
        }

        private IDictionary<string, string> m_where;
        public IDictionary<string, string> Wheres
        {
            get
            {
                if (m_where == null) m_where = new Dictionary<string, string>();
                return m_where;
            }
            set { m_where = value; }//deserialize
        }
        private List<string> m_orderBy;
        public List<string> OrderBys
        {
            get
            {
                if (m_orderBy == null) m_orderBy = new List<string>();
                return m_orderBy;
            }
            set { m_orderBy = value; }//deserialize
        }

        private string queryObject;
        /// <summary>
        /// 实体名和别名,逗号分隔多个实体。 如: Customer cust,...
        /// </summary>
        private string QueryObject
        {
            get { return queryObject; }
            set 
            { 
                queryObject = value;
                if (queryObject!=null && queryObject.IndexOf(',') < 0)//only one entity
                {
                    int iAlias = queryObject.LastIndexOf(' ');
                    if (iAlias > 2)
                        this.alias = queryObject.Substring(iAlias + 1).Trim();//����
                }
            }
        }

        /// <summary>
        /// 查询别名 QueryObject=User u, 则别名为" u. "
        /// </summary>   
        private string alias = string.Empty;

        /// <summary>
        /// 必须为带别名的属性：在Find拦截时根据此属性(alias.Property)过滤，如i.Id , 还可以是CreatedOffice,CreatedBy
        /// </summary>
        public string AclProperty
        {
            get;
            set;
        }

        private string m_countField;
        /// <summary>
        /// 默认将采用count(*)统计
        /// </summary>
        public string CountField
        {
            get
            {
                if (m_countField == null) m_countField = "*";
                return m_countField;
            }
            set { m_countField = value; }
        }

        /// <summary>
        /// 自定义SQL/HQL语句,除where部分
        /// </summary>
        public string CustomSQL
        {
            get;
            set;
        }

        /// <summary>
        /// 简单SQL分离为field +from +where +groupby +orderby
        /// </summary>
        private string groupBy
        {
            get;
            set;
        }
	

        /// <summary>
        /// NamedQuery/SP
        /// 注意：Parameters的参数顺序，请务必与存储过程内部顺序相同!
        /// </summary>
        public string NamedQuery
        {
            get;
            set;
        }

        ///// <summary>
        ///// HQL without OrderBy string
        ///// </summary>
        //public string ToHQLString()
        //{
        //    if (!string.IsNullOrEmpty(this.AclProperty))
        //        FireAclInjection();

        //    StringBuilder sb = new StringBuilder();
        //    if (this.CustomSQL == null)//no statement customized,let's build it.
        //    {
        //        if (this.QueryObject != null && this.QueryObject.IndexOf(" from ", StringComparison.InvariantCultureIgnoreCase) < 0)
        //            sb.Append(" from ");
        //        sb.Append(this.QueryObject);
        //        if(this.Where.Count>0)
        //            sb.Append(" where 1=1");
        //    }
        //    else
        //        BuildCustomSQL(sb);

        //    BuildWhere(sb);
        //    return sb.ToString();
        //}

        /// <summary>
        /// SQL without OrderBy string
        /// </summary>
        public string ToSQLString()
        {
            if(this.NamedQuery!=null)
                return this.NamedQuery;
                
            StringBuilder sb = new StringBuilder();
            if (this.CustomSQL == null)//no statement customized,let's build it.
            {
                if (this.QueryObject!=null && this.QueryObject.IndexOf(" from ", StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    sb.Append("select * from ");
                }
                    
                sb.Append(this.QueryObject);
                if (this.Wheres.Count > 0)
                    sb.Append(" where 1=1");
            }else
                BuildCustomSQL(sb);          

            BuildWhere(sb);
            return sb.ToString();
        }
        private void BuildCustomSQL(StringBuilder sb)
        {
            if(!string.IsNullOrEmpty(this.SelectFields))
            {
                this.CustomSQL = this.CustomSQL.Replace("select *", "select " + this.SelectFields);
            }

            int i = this.CustomSQL.IndexOf("order by", StringComparison.CurrentCultureIgnoreCase);
            if (i > 0)//将Order By从SQL拆开
            {
                if (this.OrderBys.Count == 0)
                    this.OrderBys.Add(this.CustomSQL.Substring(i + 8));
                this.CustomSQL = this.CustomSQL.Substring(0, i);
            }
            sb.Append(this.CustomSQL);

            if (this.Wheres.Count > 0 && this.CustomSQL.IndexOf(" where ", StringComparison.CurrentCultureIgnoreCase) < 0)//总是启用一个where根
                sb.Append(" where 1=1");
        }
        private void BuildWhere(StringBuilder sb)
        {
            //where fragment
            if (this.Wheres.Count > 0)
            {
                foreach (string con in this.Wheres.Values)
                    sb.Append(" ").Append(con);
            }
            if (!string.IsNullOrEmpty(groupBy))
            {
                sb.Append(" group by ");
                sb.Append(groupBy);
            }
        }

        /// <summary>
        /// OrderBy is not supported when 'COUNT' query. so seperate it.
        /// </summary>
        public string ToOrderBy()
        {
            //order by fragment
            if (this.OrderBys.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" order by ");
                foreach (string ord in this.OrderBys)
                    sb.Append(ord).Append(",");
                sb.Remove(sb.Length - 1, 1);//last comm
                return sb.ToString();
            }
            return string.Empty;
        }

        protected static readonly string EQ_EXPRESSION="and {0}{1} = "+StatementParser.PREFIX+"{2}";

        /// <summary>
        /// 控件名为 prm_Name_LLK_u_ 分别为prm_{字段名}_{匹配类型?}_{别名?}_. 匹配及别名可有可无
        /// 如：prm_Name_u_ , prm_Name_GT_ , prm_Time_u_V2 都有效
        /// </summary>
        public QueryInfo AddParam(object dictionary)
        {
            if (null == dictionary)
                return this;

            AddParam((IDictionary)dictionary);
            //if(typeof(IDictionary).IsAssignableFrom(dictionary.GetType()))
            //    AddParam((IDictionary)dictionary);
            //else
            //{
            //    //Object to dictionary
            //    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(dictionary.GetType());
            //    Dictionary<string, object> di = new Dictionary<string, object>(props.Count);
            //    foreach(PropertyDescriptor ps in props)
            //    {
            //        di.Add(ps.Name, ps.GetValue(dictionary));
            //    }
            //    AddParam(di);
            //}
            return this;
        }

        static readonly Regex dtRegex = new Regex(@"^\d{2,4}-\d{1,2}-\d{1,2}", RegexOptions.Compiled);
        static readonly Regex paramRegex = new Regex(@"^(?<NAME>\w*?){1}(_(?<PAT>\w{2,3}))?(_(?<ALIAS>\w))?$", RegexOptions.Compiled);
        private object ChangeDbType(object valOld)
        {
            string val = valOld as string;
            if (val != null)
            {
                if (dtRegex.Match(val).Success)
                    return DateTime.Parse(val);
            }
            return valOld;
        }
        /// <summary>
        /// 属性转列名
        /// </summary>
        protected string TryGetMappingColumn(string propertyName,bool isAddMark=true)
        {
            Type t=this.GetMappingType();
            string col;
            if (MappingInfo.GetMappingInfo(t).AllProperties.TryGetValue(propertyName, out col))
                return isAddMark?string.Format("`{0}`", col):col;
            
            return isAddMark?string.Format("`{0}`", propertyName): propertyName; 
        }

        public QueryInfo RemoveParam(string paramName)
        {
            this.Parameters.Remove(paramName);
            this.Wheres.Remove(paramName);
            return this;
        }
        /// <summary>
        /// patten: {Field}_{VAL} => info仅加入参数值, 不加入where条件
        /// </summary>
        /// <param name="ps"></param>
        private void AddParam(IDictionary ps)
        {
            if (ps == null) return;
            foreach (string key in ps.Keys)
            {
                if (ps[key] != null)
                {
                    object val = ps[key];//参数值
                    //val = val.Trim();
                    if (val!=null && !string.Empty.Equals(val))
                    {
                        Match m = paramRegex.Match(key);//{Field}_{LLK}_{u}
                        string sField = m.Groups["NAME"].Value;
                        string alias = m.Groups["ALIAS"].Value;
                        string patten = m.Groups["PAT"].Value;
                        if (!string.IsNullOrEmpty(patten))//'LLK,GEQ'
                        {
                            if (patten.Equals("VAL"))//仅加入值
                            {
                                Parameters.Add(sField, ChangeDbType(val));
                                continue;
                            }
                            
                            bool bChangedDbType = false;//参数可能需要转换类型
                            string sParam = key;//参数名
                            StringBuilder exp = new StringBuilder("and ");//where query expression

                            #region Like 逐字段组装
                            if (patten.EndsWith("LK"))
                            {
                                string[] fields = sField.Split('_');
                                if (fields.Length < 2)
                                {
                                    exp.AppendFormat("{{0}}{1} like "+StatementParser.PREFIX+"{1}", TryGetMappingColumn(sField,false), sParam);//fieldName like :ParamName
                                }
                                else//multiple criteria
                                {
                                    exp.Append("(");

                                    sParam = alias + fields[0] + fields.Length.ToString();//重新设置参数名
                                    for (int i = 0; i < fields.Length; i++)//遍历每个字段
                                    {
                                        exp.AppendFormat("{{0}}{1} like "+StatementParser.PREFIX+"{2} or ", TryGetMappingColumn(fields[i],false), sParam);//fieldName like :ParamName
                                    }
                                    exp.Remove(exp.Length - 4, 4);
                                    exp.Append(")");
                                }
                            }
                            else
                            {
                                sField = TryGetMappingColumn(sField,false);
                            }
                            #endregion
                            switch (patten)
                            {
                                #region Like类型
                                case "LLK"://Left like?
                                    val = "%" + val.ToString();
                                    break;
                                case "RLK"://Right like?
                                    val = val.ToString() + "%";
                                    break;
                                case "LK"://Left like?
                                    val = "%" + val.ToString() + "%";
                                    break;
                                #endregion

                                #region 大于，小于
                                case "GT":
                                    exp.AppendFormat("{{0}}{1} > "+StatementParser.PREFIX+"{2}", sField, sParam);
                                    bChangedDbType = true;
                                    break;
                                case "GEQ":
                                    exp.AppendFormat("{{0}}{1} >= "+StatementParser.PREFIX+"{2}", sField, sParam);
                                    bChangedDbType = true;
                                    break;
                                case "LT":
                                    exp.AppendFormat("{{0}}{1} < "+StatementParser.PREFIX+"{2}", sField, sParam);
                                    bChangedDbType = true;
                                    break;
                                case "LEQ":
                                    exp.AppendFormat("{{0}}{1} <= "+StatementParser.PREFIX+"{2}", sField, sParam);
                                    string date = val as string;//日期型，无时间?
                                    if (date != null && date.IndexOf('-') == 4 && date.IndexOf(' ') < 0)
                                        val = date + " 23:59:59";
                                    bChangedDbType = true;
                                    break;
                                case "NEQ":
                                    exp.AppendFormat("{{0}}{1} <> "+StatementParser.PREFIX+"{2}", sField, sParam);
                                    bChangedDbType = true;
                                    break;
                                case "EQ":
                                    exp.AppendFormat("{{0}}{1} = "+StatementParser.PREFIX+"{2}", sField, sParam);
                                    bChangedDbType = true;
                                    break;
                                #endregion

                                case "NVL":
                                    exp.AppendFormat("{{0}}{1} is null", sField);
                                    val = null;
                                    break;

                                case "IN"://==> and a.Id in ('001','002')
                                    string s = val.ToString();
                                    if (s.IndexOf("'") > 0) s = "SQL_INJECT?";
                                    exp.AppendFormat("{{0}}{1} in ('{2}')", sField, s.Replace(",", "','"));
                                    val = null;
                                    break;

                                case "STR"://原样加入参数Value， 不做类型转换，不做语句处理
                                    sParam = sField;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException("无效的where关系‘" + key + "’， 无法匹配LK/IN/NVL/GEQ等模式，注意alias应为一个字符.");
                            }
                            
                            AddParam(alias,sParam,sParam, (bChangedDbType && val is string) ? ChangeDbType(val) : val, exp.ToString());
                        }
                        else
                            AddParam(TryGetMappingColumn(sField,false), ChangeDbType(val));
                    }
                }//val not null
                else
                    AddParam(key, null,EQ_EXPRESSION);

            }//foreach
        }
        /// <summary>
        /// 为语句提供参数值,若非NamedQuery,并且CustomSQL和where中均不存在此参数,则自动添加 and {0}=:{0}
        /// </summary>
        public QueryInfo AddParam(string namedParam, object value)
        {
            if(this.CustomSQL!=null && this.CustomSQL.IndexOf(".")>0)//xml?
                this.Parameters.Add(namedParam, value);
            else
                AddParam(namedParam, value, EQ_EXPRESSION);
            return this;
        }
        /*
        /// <summary>
        /// 仅加入参数(不为null/Empty)，不添加where（用于动态语句）
        /// </summary>
        public QueryInfo AddParamOnly(string namedParam, object value)
        {
            if (value != null && !string.Empty.Equals(value))
                this.Parameters.Add(namedParam, value);
            return this;
        }
        /// <summary>
        /// 仅当参数值不为null/Empty时，添加参数和条件
        /// </summary>
        public QueryInfo TryAddParam(string namedParam, object value)
        {
            if (value != null && !string.Empty.Equals(value))
                AddParam(namedParam, value);
            return this;
        }
        /// <summary>
        /// 仅当参数值不为null/Empty时，添加参数和条件
        /// </summary>
        public QueryInfo TryAddParam(string namedParam, object value, string expression)
        {
            if (value != null && !string.Empty.Equals(value))
                AddParam(namedParam, value, expression);
            return this;
        }*/
        /// <summary>
        /// 为语句提供参数值,若value=null,value=>DBNull.Value
        /// </summary>
        public QueryInfo AddParam(string namedParam, object value, string expression)
        {
            string pAlias = this.alias;
            int i = namedParam.IndexOf(".");
            if (i > 0)//参数名带点号
            {
                string[] p = namedParam.Split('.');
                pAlias = p[0];//实体别名
                namedParam = p[1];//参数名
            }
            if (expression.IndexOf("("+StatementParser.PREFIX) > 0)// column in (:values) => column in ('...','..')
            {
                string[] o = value as string[];
                if (o != null && o.Length > 0)
                {
                    expression = expression.Replace(StatementParser.PREFIX + namedParam, "'" + string.Join("','", o) + "'");
                    value = null;
                }
            }
            AddParam(pAlias, namedParam,namedParam, value, expression);//参数名、列名一致
            return this;
        }

        /// <summary>
        /// 对空值、NULL值，不添加任何语句
        /// </summary>
        protected void AddParam(string alias,string colName,string namedParam, object value, string expression)
        {
            if(value==null || string.Empty.Equals(value))//by crabo 2016.09.26
                return;

            if(namedParam==null)//使用自增的动态参数名？
                namedParam=colName+Parameters.Count.ToString();
            
            if (this.NamedQuery != null)//sp?
            {
                if (Parameters.ContainsKey(namedParam))
                    throw new ArgumentException("试图重复添加同一参数名:" + namedParam);
                else
                    Parameters.Add(namedParam, value);

                return;
            }

            if (value != null)
            {
                if (Parameters.ContainsKey(namedParam))
                    throw new ArgumentException("试图重复添加同一参数名:" + namedParam);
                else
                    Parameters.Add(namedParam, value);
            }
            else if (expression.IndexOf(StatementParser.PREFIX + namedParam) > 0 //参数名在语句中已存在
                || expression.IndexOf(StatementParser.PREFIX+"{2}") > 0)//参数占位符{2}在语句中已存在
                    expression = "and {0}{1} is null";

            if (!string.IsNullOrEmpty(alias))
                alias += ".";
            if ((this.CustomSQL == null || this.CustomSQL.IndexOf(StatementParser.PREFIX + namedParam) < 0) //not in sql text
                && !Wheres.ContainsKey(namedParam)) //not in where
                this.Wheres.Add(namedParam, string.Format(expression, alias,colName,namedParam));//add to where
        }
        private IDictionary<string,object> m_parameters;
        /// <summary>
        /// 输入、输出参数表
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get
            {
                if (m_parameters == null) m_parameters = new Dictionary<string, object>();
                return m_parameters; 
            }
            set 
            {
                m_parameters = value;
            }
        }

        [DataMember]
        /// <summary>
        /// Query result to be returned!
        /// </summary>
        public List<object> List
        {
            get;
            set;
        }

        /// <summary>
        /// Type to mapping result,can be typeof(MyEntity) or typeof(MyEntity).AssemblyQualifiedName
        /// </summary>
        public object MappingType
        {
            get;
            set;
        }

        /// <summary>
        /// 重复使用实例时,需重新初始化
        /// </summary>
        public void Init()
        {
            this.QueryObject = null;
            this.startRecord = 0;
            this.pageSize = 0;
            this.totalCount = 0;
            this.alias = string.Empty;
            this.Wheres.Clear();
            this.OrderBys.Clear();
            this.CountField = null;
            this.CustomSQL = null;
            this.NamedQuery = null;
            this.Parameters.Clear();
            this.List = null;
            this.MappingType = null;
        }

        public object Clone()
        {
            QueryInfo info = new QueryInfo();
            IEnumerator<KeyValuePair<string,object>> enumer = this.Parameters.GetEnumerator();
            while (enumer.MoveNext())
                info.Parameters.Add(enumer.Current.Key, enumer.Current.Value);

            IEnumerator<KeyValuePair<string, string>> senumer = this.Wheres.GetEnumerator();
            while (senumer.MoveNext())
                info.Wheres.Add(senumer.Current.Key, senumer.Current.Value);

            info.CountField = this.CountField;
            info.CustomSQL = this.CustomSQL;
            info.NamedQuery = this.NamedQuery;
            info.OrderBys = this.OrderBys;
            info.pageSize = this.pageSize;
            info.QueryObject = this.QueryObject;
            info.startRecord = this.startRecord;
            info.totalCount = this.totalCount;
            info.MappingType = this.MappingType;
            return info;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.Append("\tTotalCount:");
            sb.Append(this.totalCount);
            sb.Append(Environment.NewLine);

            if(this.NamedQuery==null && this.CustomSQL==null && this.QueryObject==null){
                Type t=this.GetMappingType();
                this.QueryObject = MappingInfo.GetMappingInfo(t).Table;
            }
            sb.Append(this.NamedQuery ?? (this.ToSQLString() + this.ToOrderBy()));
            IEnumerator<KeyValuePair<string,object>> enumer = this.Parameters.GetEnumerator();
            while (enumer.MoveNext())//输出参数值
            {
                sb.Append(Environment.NewLine).Append("\t").Append(StatementParser.PREFIX)
                    .Append(enumer.Current.Key).Append("=").Append(enumer.Current.Value);
            }
            
            return sb.ToString();
        }

        public virtual Type GetMappingType(){
            Type t = this.MappingType as Type;
            if (t == null){
                if(this.MappingType!=null)
                    t = Type.GetType((string)this.MappingType);
            }
            return t;
        }

        #region Build SQL statement


        /// <summary>
        /// 根据Keys返回字符串: param=:param,param1=:param1....
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetSQLPartialUpdate<T>(IEnumerable keys)
        {
            var mapping = MappingInfo.GetMappingInfo<T>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", mapping.Table);
            foreach (string key in keys)
            {
                string col = null;
                if(mapping==null) mapping.AllProperties.TryGetValue(key,out col);
                if(col==null) col=key;

                sb.Append(col);
                sb.Append("=");
				sb.Append(StatementParser.PREFIX);
                sb.Append(key);
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            //sb.AppendFormat(" FROM {0} t0 ", mapping.Table); ORACLE NOT SUPPORT UPDATE/FROM
            return sb.ToString();
        }


        #endregion
    }
}
