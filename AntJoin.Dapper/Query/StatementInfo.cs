using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using System.ComponentModel;
using System.Linq;

namespace AntJoin.Dapper
{
internal class StatementInfo
    {
        #region Regex
        Regex regexCriteria;
        Regex RegexCriteria
        {
            get
            {
                if (regexCriteria == null)
                    regexCriteria = new Regex(@"{([^{}\[\]]+)(\[(\S*)\])*}");
                return regexCriteria;
            }
        }
        Regex regexParamName;
        Regex RegexParamName
        {
            get
            {
                if (regexParamName == null)
                    regexParamName = new Regex(StatementParser.PREFIX+"(\\w*)");
                return regexParamName;
            }
        }

        
        #endregion

        public StatementInfo(string key,string sql)
        {
            this.Key = key;

            int i = sql.IndexOf("{");
            if (i > 0)
            {
                List<string> conditions = new List<string>();//Detection
                List<string> paramNames = new List<string>();

                this.Sql = sql.Substring(0, i);//sql without replacement criteria.

                var mt = this.RegexCriteria.Matches(sql);
                foreach (Match m in mt)//every {and Field=:Param}
                {
                    Criteria where = new Criteria();
                    #region Test condition?
                    if (m.Groups[2].Success)//condition? {and p1=:p1 [p1&&p2]}==> both p1&p2 is not null then add criteria
                    {
                        string con = m.Groups[3].Value;
                        if (con.Length > 0)
                        {
                            if (con.IndexOf('=') > 0) where.HasComparator = true;//需进行值比较
                            if (con.IndexOf('|') > 0)
                            {
                                where.Cons = con.Split('|');
                                where.IsOr = true;
                            }
                            else
                            {
                                where.Cons = con.Split('&');
                                where.IsOr = false;
                            }
                            conditions.AddRange(where.Cons);
                        }
                    }
                    #endregion
                    where.Criterion = m.Groups[1].Value;
                    string pName = RegexParamName.Match(where.Criterion).Groups[1].Value;
                    if (pName.Length > 0)
                    {
                        where.ParamName = pName;
                        paramNames.Add(pName);
                    }

                    where.IsInClause = where.Criterion.IndexOf("("+StatementParser.PREFIX) > 0 ||
                        where.Criterion.IndexOf(" in "+StatementParser.PREFIX, StringComparison.CurrentCultureIgnoreCase) > 0;

                    DynamicCriterias.Add(where);
                }
                //仅作为条件标示，从不在Where语句出现
                ConditionOnlyFlags = conditions.Select(m => m.IndexOf('=') > 0 ? m.Substring(0, m.IndexOf('=')) : m)
                    .Where(m => !paramNames.Contains(m))
                    .ToArray();
            }
            else
                this.Sql = sql;
        }
        public string Key { get; set; }
        public string Sql { get; set; }
        public IEnumerable<string> ConditionOnlyFlags { get; set; }

        List<Criteria> criterias;
        public List<Criteria> DynamicCriterias
        {
            get
            {
                if (criterias == null)
                    criterias = new List<Criteria>();
                return criterias;
            }
        }
    }

    internal class Criteria
    {
        /// <summary>
        /// null: 无条件, IsOr=true:或， IsOr=false:与(and)
        /// </summary>
        public bool? IsOr { get; set; }
        public bool IsInClause { get; set; }
        /// <summary>
        /// 参数值是Array，必须变换为String
        /// </summary>        
        public string[] Cons { get; set; }
        //匹配条件:Name=='01'
        public bool HasComparator { get; set; }
        public string ParamName { get; set; }
        public string Criterion { get; set; }
    }
}