using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Linq.Expressions;

namespace AntJoin.Dapper
{
    public class QueryInfo<T> : QueryInfo
    {
        public QueryInfo() : base()
        {
        }

        /// <summary>
        /// 取几条数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public QueryInfo<T> Take(int pageSize)
        {
            //base.TotalCount = 1;
            base.PageSize = pageSize;
            return this;
        }
        public QueryInfo<T> Skip(int startRecord)
        {
            base.StartRecord = startRecord;
            return this;
        }
        public virtual QueryInfo<T> From(string sql)
        {
            if (sql == null)
                return this;
            this.CustomSQL = sql;
            return this;
        }
        /// <summary>
        /// 限制输出为指定的输出字段
        /// </summary>
        public QueryInfo<T> Select(params Expression<Func<T, object>>[] keys)
        {
            if (keys.Length > 0)
            {
                var cols = new List<string>(keys.Length);
                foreach (var exp in keys)
                {
                    cols.Add(GetColumnName(exp));
                }
                this.SelectFields = string.Join(",", cols);
            }
            return this;
        }
        string GetColumnName(Expression<Func<T, object>> exp, bool isAddMark = true)
        {
            MemberExpression body = exp.Body as MemberExpression;
            if (body == null)
                body = ((UnaryExpression)exp.Body).Operand as MemberExpression;

            string prop = body.Member.Name;
            return TryGetMappingColumn(prop, isAddMark);
        }

        /// <summary>
        /// 等值比较where
        /// </summary>
        public new QueryInfo<T> AddParam(string key, object value)
        {
            base.AddParam(key, value);
            return this;
        }
        /// <summary>
        /// 等值比较where
        /// </summary>
        public QueryInfo<T> AddParam(Expression<Func<T, object>> key, object value)
        {
            base.AddParam(GetColumnName(key, false), value);
            return this;
        }

        /// <summary>
        /// in 操作
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public QueryInfo<T> AddParamIn<T2>(Expression<Func<T, object>> key, IEnumerable<T2> value)
        {
            base.AddParamIn(GetColumnName(key, false), value);
            return this;
        }
        /// <summary>
        /// in 操作 (SQL)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public QueryInfo<T> AddParamIn(Expression<Func<T, object>> key, string sql)
        {
            base.AddParamIn(GetColumnName(key, false), sql);
            return this;
        }

        /// <summary>
        /// in 操作
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public QueryInfo<T> AddParamNotIn<T2>(Expression<Func<T, object>> key, IEnumerable<T2> value)
        {
            base.AddParamNotIn(GetColumnName(key, false), value);
            return this;
        }
        /// <summary>
        /// in 操作 (SQL)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public QueryInfo<T> AddParamNotIn(Expression<Func<T, object>> key, string sql)
        {
            base.AddParamNotIn(GetColumnName(key, false), sql);
            return this;
        }


        /// <summary>
        /// Name > @Name
        /// </summary>
        public QueryInfo<T> AddParamGT(Expression<Func<T, object>> key, object value)
        {
            base.AddParam(null, GetColumnName(key, false), null, value, EQ_EXPRESSION.Replace("=", ">"));
            return this;
        }
        /// <summary>
        /// Name >= @Name
        /// </summary>
        public QueryInfo<T> AddParamGEQ(Expression<Func<T, object>> key, object value)
        {
            base.AddParam(null, GetColumnName(key, false), null, value, EQ_EXPRESSION.Replace("=", ">="));
            return this;
        }
        /// <summary>
        /// Name < @Name
        /// </summary>
        public QueryInfo<T> AddParamLT(Expression<Func<T, object>> key, object value)
        {
            base.AddParam(null, GetColumnName(key, false), null, value, EQ_EXPRESSION.Replace("=", "<"));
            return this;
        }
        /// <summary>
        /// Name <= @Name
        /// </summary>
        public QueryInfo<T> AddParamLEQ(Expression<Func<T, object>> key, object value)
        {
            base.AddParam(null, GetColumnName(key, false), null, value, EQ_EXPRESSION.Replace("=", "<="));
            return this;
        }
        /// <summary>
        /// Name <> @Name
        /// </summary>
        public QueryInfo<T> AddParamNEQ(Expression<Func<T, object>> key, object value)
        {
            base.AddParam(null, GetColumnName(key, false), null, value, EQ_EXPRESSION.Replace("=", "<>"));
            return this;
        }
        public QueryInfo<T> AddParamLK(Expression<Func<T, object>> key, object value)
        {
            base.AddParam(null, GetColumnName(key, false), null, value, EQ_EXPRESSION.Replace("=", "like"));
            return this;
        }
        /*
        /// <summary>
        /// 仅当参数值不为null/Empty时，添加参数和条件
        /// </summary>
        public QueryInfo<T> TryAddParam(Expression<Func<T, object>> key, object value)
        {
            this.TryAddParam(GetColumnName(key), value);
            return this;
        }
        public QueryInfo<T> TryAddParam(Expression<Func<T, object>> key, object value,string exp)
        {
            this.TryAddParam(GetColumnName(key), value,exp);
            return this;
        }
        
        /// <summary>
        /// 仅写入参数值
        /// </summary>
        public QueryInfo<T> AddValue(Expression<Func<T, object>> key, object value)
        {
            this.AddParam(GetColumnName(key), value);
            return this;
        }*/
        /// <summary>
        /// 按表达式赋值where
        /// </summary>
        /// <param name="expression">如： and {0} = :{0}</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public QueryInfo<T> AddParam(Expression<Func<T, object>> key, object value, string expression)
        {
            this.AddParam(GetColumnName(key, false), value, expression);
            return this;
        }

        public QueryInfo<T> OrderBy(Expression<Func<T, object>> key)
        {
            this.OrderBy(GetColumnName(key));
            return this;
        }

        public QueryInfo<T> OrderByDesc(Expression<Func<T, object>> key)
        {
            this.OrderBy(GetColumnName(key) + " desc");
            return this;
        }

        public QueryInfo<T> OrderByExt(string columnName, bool isDesc)
        {
            OrderBy(columnName + (isDesc ? " desc" : ""));
            return this;
        }

        public QueryInfo<T> GroupBy(Expression<Func<T, object>> key)
        {
            this.GroupBy(GetColumnName(key));
            return this;
        }

        List<T> m_listOfT;
        /// <summary>
        /// Query result to be returned!
        /// </summary>
        public new List<T> List
        {
            get
            {
                if (m_listOfT == null && base.List != null)
                    return base.List.Cast<T>().ToList();
                return m_listOfT;
            }
            set
            {
                m_listOfT = value;
            }
        }

        /// <summary>
        /// 将where条件 加入到update语句中
        /// </summary>
        /// <param name="di">动态update目标</param>
        /// <returns></returns>
        public string GetSQLPartialUpdate(System.Collections.IDictionary di)
        {
            string update = QueryInfo.GetSQLPartialUpdate<T>(di.Keys);

            if (this.Wheres.Count > 0)
            {
                update += " where 1=1";
                int i = 0;
                foreach (var kp in this.Wheres)
                {
                    var name = "p" + i;
                    i++;

                    update = update + " " //为update语句逐条添加
                        + kp.Value.Replace(StatementParser.PREFIX + kp.Key, StatementParser.PREFIX + name);//参数重命名

                    if (this.Parameters.ContainsKey(kp.Key))//设置where部分。 更换参数名为p1
                    {
                        object val = this.Parameters[kp.Key];//替换参数名
                        this.Parameters.Add(name, val);
                        this.Parameters.Remove(kp.Key);
                    }
                }

            }

            var enumer = di.GetEnumerator();
            while (enumer.MoveNext())//设置set部分的参数
                this.Parameters.Add((string)enumer.Key, enumer.Value);

            return update;
        }

        public override Type GetMappingType()
        {
            Type t = base.GetMappingType();
            if (t == null)
                base.MappingType = t = typeof(T);
            return t;
        }
    }
}
