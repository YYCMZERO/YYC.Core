using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Dappers.Cache;
using Dappers.Cfg;
using Dappers.Mapping;
using System.Reflection;

namespace Dappers.Session
{
    public class DapperSession : ISqlSession
    {
        public DapperSession(IDao dao)
        {
            this.dao = dao;

            //if(ConfigManager.GetRootConfig("DB_ADAPTERS_WATCH")!=null){
            //    XmlChangeWatcher.Instance.Watch(
            //        ConfigManager.GetRootConfig("DB_ADAPTERS_WATCH") );
            //}
        }
        IDao dao;
        public IDao Dao
        {
            get
            {
                if (dao == null)
                    throw new System.ArgumentNullException("未指定DapperSession依赖的IDao对象.");
                return dao;
            }
            set { dao = value; }
        }
        public void TxBegin()
        {
            Dao.TxBegin();
        }
        public void TxRollback()
        {
            Dao.TxRollback();
        }
        public void TxCommit()
        {
            Dao.TxCommit();
        }
        /// <summary>
        /// close connection and commint Tx
        /// </summary>
        public void Close()
        {
            Dao.Close();
        }

        #region 判断该实体是否加入缓存读写

        /// <summary>
        /// 判断该实体是否加入缓存读写
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TableAttribute IsCacheReadWrite(Type type)
        {
            var props = type.GetTypeInfo().GetCustomAttributes(typeof(TableAttribute), true).ToArray();
            TableAttribute table = null;
            if (props != null && props.Length > 0)
                table = props[0] as TableAttribute;
            if (table.IsCacheReadWrite)
                return table;
            return null;
        }

        #endregion

        #region 获取主键名称

        /// <summary>
        /// 获取主键名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetPrimaryKey(Type type)
        {
            string primaryKey = string.Empty;

            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.GetProperty);
            foreach (System.Reflection.PropertyInfo p in properties)
            {
                if (p.PropertyType.Namespace.Equals("System") || p.PropertyType.IsEnum())//only primitive types
                {
                    ColumnAttribute col = null;
                    object[] attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false).ToArray();
                    if (attrs.Length > 0) col = attrs[0] as ColumnAttribute;
                    if (col != null)
                    {
                        if (col.IsPrimaryKey)
                        {
                            primaryKey = col.Name;
                            break;
                        }
                    }
                }
            }
            if (primaryKey == string.Empty)
            {
                throw new ArgumentException("该表格未设置主键！");
            }
            return primaryKey;
        }

        #endregion

        #region 从redis缓存中移除该数据

        /// <summary>
        /// 从redis缓存中移除该数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        private void DeleteCacheInfo(Type type, object id)
        {
            if (id == null) return;

            bool isDeleted = false;

            var key = "Open.Model." + type.Name + "_-1#?#Open.Model." + type.Name + "#" + id.ToString();

            if (StackExchangeHelper.KeyExists(key))
            {
                while (!isDeleted)
                {
                    if (StackExchangeHelper.KeyExists(key))
                    {
                        if (StackExchangeHelper.KeyDelete(key))
                        {
                            isDeleted = true;
                        }
                    }
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// 从redis缓存中移除该数据
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteCacheInfo(object obj)
        {
            Type type = obj.GetType();
            string id = String.Empty;
            TableAttribute table = IsCacheReadWrite(type);
            if (table != null)
            {
                var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.GetProperty);
                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    if (p.PropertyType.Namespace.Equals("System") || p.PropertyType.IsEnum())//only primitive types
                    {
                        ColumnAttribute col = null;
                        object[] attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false).ToArray();
                        if (attrs.Length > 0) col = attrs[0] as ColumnAttribute;
                        if (col != null)
                        {
                            if (col.IsPrimaryKey)
                            {
                                id = p.GetValue(obj, null).ToString();
                                break;
                            }
                        }
                    }
                }
                DeleteCacheInfo(type, id);
            }
        }

        /// <summary>
        /// 从redis缓存中移除该组数据
        /// </summary>
        /// <param name="objs"></param>
        private void DeleteCacheInfo<T>(IList<T> objs)
        {
            if (objs == null || objs.Count == 0) return;

            List<object> ids = new List<object>();
            Type type = typeof(T);
            string value = String.Empty;

            TableAttribute table = IsCacheReadWrite(type);

            if (table != null)
            {
                var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.GetProperty);
                foreach (var obj in objs)
                {
                    foreach (System.Reflection.PropertyInfo p in properties)
                    {
                        if (p.PropertyType.Namespace.Equals("System") || p.PropertyType.IsEnum())//only primitive types
                        {
                            ColumnAttribute col = null;
                            object[] attrs = p.GetCustomAttributes(typeof(ColumnAttribute), false).ToArray();
                            if (attrs.Length > 0) col = attrs[0] as ColumnAttribute;
                            if (col != null)
                            {
                                if (col.IsPrimaryKey)
                                {
                                    ids.Add(p.GetValue(obj, null).ToString());
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (var id in ids)
                {
                    DeleteCacheInfo(type, id);
                }
            }
        }

        #endregion

        public bool Delete<T>(Object id)
        {
            if (id == null) return false;

            Type type = typeof(T);

            if (IsCacheReadWrite(type) != null)
            {
                DeleteCacheInfo(type, id);
            }

            MappingInfo info = MappingInfo.GetMappingInfo<T>();
            return Dao.Execute(
                    info.Delete,
                    new Dictionary<string, object>() { { GetPrimaryKey(type), id } }
                ) == 1;
        }

        public int Delete(Type type, IDictionary<string, object> param)
        {
            bool hasId = false;
            string primaryKey = GetPrimaryKey(type);
            string sql = MappingInfo.GetSqlStatementByType(type, "Delete");
            sql = sql.Substring(0, sql.IndexOf(" where ", StringComparison.CurrentCultureIgnoreCase) + 7);

            string where = "";

            foreach (string key in param.Keys)
            {
                if ((String.Compare(key, primaryKey, true) == 0))
                {
                    hasId = true;
                    DeleteCacheInfo(type, param[key]);
                }

                where += string.Format(" and {0}={1}{0}", key, StatementParser.PREFIX);
            }
            if (IsCacheReadWrite(type) != null && !hasId)
            {
                throw new ArgumentException("该表格有加入缓存读写，但当前查询信息未包括主键字段！");
            }
            sql = sql + where.Substring(5);
            return Dao.Execute(
                    sql,
                    param
                );
        }

        public bool Delete(Object obj)
        {
            if (obj == null) return false;

            DeleteCacheInfo(obj);

            string sql = MappingInfo.GetSqlStatementByType(obj.GetType(), "Delete");
            return Dao.Execute(
                    sql,
                    obj
                ) == 1;
        }

        public bool Insert(Object obj, string extTableName = null)
        {
            if (obj == null) return false;

            string sql = MappingInfo.GetSqlStatementByType(obj.GetType(), "Insert", extTableName);
            return Dao.Execute(
                    sql,
                    obj
                ) == 1;
        }

        public bool Insert(Object obj, out Int32 lastInsertId, string extTableName = null)
        {
            lastInsertId = -1;
            if (obj == null) return false;

            string sql = MappingInfo.GetSqlStatementByType(obj.GetType(), "Insert", extTableName);
            if (Dao.ConnectionManager.ConnectionTypeName.StartsWith("MySql."))
                sql += ";SELECT LAST_INSERT_ID();";

            lastInsertId = Convert.ToInt32(
                Dao.Query<long>(
                    sql,
                    obj
                ).First()
            );

            return lastInsertId > 0;
        }

        static Regex REGEX_GET_PARAM = new Regex("(" + StatementParser.PREFIX + "\\w*)", RegexOptions.Multiline);
        public int Insert<T>(IList<T> li, string extTableName = null)
        {
            int affectedRows = 0;
            if (li == null || li.Count == 0) return -1;
            string sql = MappingInfo.GetSqlStatementByType(li[0].GetType(), "Insert", extTableName);

            List<string> paramNames = new List<string>();
            foreach (Match m in REGEX_GET_PARAM.Matches(sql))
                paramNames.Add(m.Value.Substring(1));

            sql = REGEX_GET_PARAM.Replace(sql, "$1#");//为所有参数加入#号
            StringBuilder sb = new StringBuilder(sql);

            string values = sql.Substring(sql.IndexOf(" values", StringComparison.CurrentCultureIgnoreCase) + 7);

            IDictionary<string, Object> param = new Dictionary<string, Object>();
            for (int i = 0; i < li.Count; i++)
            {                              
                if (sb.ToString().Length > 100000) //限制sql字符数
                {
                    affectedRows += Dao.Execute(sb.ToString(), param);

                    sb = new StringBuilder(sql);
                    param = new Dictionary<string, Object>();

                    sb.Replace("#", i.ToString());                    
                } 
                else if (i == 0)
                {
                    sb.Replace("#", i.ToString());                 
                }
                else
                {
                    sb.AppendLine(",")
                      .Append(values.Replace("#", i.ToString()));                    
                }

                foreach (string propname in paramNames)
                {
                    param.Add(propname + i,
                        Dappers.Context.Reflection.GetData(li[i], propname));
                }
            }

            if (sb.ToString().Length > 0)
            {
                affectedRows += Dao.Execute(sb.ToString(), param);

                sb = new StringBuilder(sql);
                param = new Dictionary<string, Object>();
            }

            return affectedRows;
        }

        public bool Update(Object obj, string extTableName = null)
        {
            if (obj == null) return false;

            DeleteCacheInfo(obj);

            string sql = MappingInfo.GetSqlStatementByType(obj.GetType(), "Update", extTableName);
            return Dao.Execute(
                    sql,
                    obj
                ) == 1;
        }

        public int Update<T>(IList<T> li, string extTableName = null)
        {
            if (li == null || li.Count == 0) return -1;

            DeleteCacheInfo(li);

            string sql = MappingInfo.GetSqlStatementByType(li[0].GetType(), "Update", extTableName);

            List<string> paramNames = new List<string>();
            foreach (Match m in REGEX_GET_PARAM.Matches(sql))
                paramNames.Add(m.Value.Substring(1));

            sql = REGEX_GET_PARAM.Replace(sql, "$1#");//为所有参数加入#号
            StringBuilder sb = new StringBuilder();

            IDictionary<string, Object> param = new Dictionary<string, Object>(li.Count * paramNames.Count);
            for (int i = 0; i < li.Count; i++)
            {
                sb.Append(sql).AppendLine(";");
                foreach (string propname in paramNames)
                {
                    param.Add(propname + i,
                        Dappers.Context.Reflection.GetData(li[i], propname));
                }
            }

            return Dao.Execute(
                    sb.ToString(),
                    param
                );
        }

        public int Update<T>(System.Collections.IDictionary di, QueryInfo<T> info)
        {
            bool hasId = false;
            Type type = typeof(T);
            string primaryKey = GetPrimaryKey(type);
            foreach (string key in info.Parameters.Keys)
            {
                if ((String.Compare(key, primaryKey, true) == 0))
                {
                    hasId = true;
                    DeleteCacheInfo(type, info.Parameters[key]);
                }
            }
            if (IsCacheReadWrite(type) != null && !hasId)
            {
                throw new ArgumentException("该表格有加入缓存读写，但当前查询信息未包括主键字段！");
            }

            string update = info.GetSQLPartialUpdate(di);
            CheckNoParams(update, info.Parameters);

            return Dao.Execute(update, info.Parameters);
        }

        /// <summary>
        /// 执行sql（注：有加入缓存读写的表格不可以使用该方法）
        /// </summary>
        /// <param name="mapSql"></param>
        /// <param name="param">支持Dictionary类型</param>
        /// <returns></returns>
        public int Execute(string mapSql, IDictionary<string, object> param)
        {
            if (mapSql.IndexOf(' ') > 0)
                CheckSqlInject(mapSql);

            string sql = StatementParser.ParseDynamicSql(mapSql, param);
            CheckNoParams(sql, param);
            //if (sql.IndexOf(";") > 0)
            //    sql = sql.Replace("\r\n", " ");

            //Dao.TxBegin();
            return Dao.Execute(sql, param);
        }

        public int Execute(QueryInfo info)
        {
            if (IsCacheReadWrite(info.MappingType.GetType()) != null)
            {
                throw new ArgumentException("该表格有加入缓存读写，无法使用该方法！");
            }
            GetDynamicOrDefaultSql(info);
            return Dao.Execute(info);
        }

        /// <summary>
        /// update、delete语句必须提供参数才允许执行！
        /// </summary>
        void CheckNoParams(string sql, IDictionary<string, object> param)
        {
            if (sql.IndexOf("delete from", StringComparison.CurrentCultureIgnoreCase) > -1 || sql.IndexOf(" set ", StringComparison.CurrentCultureIgnoreCase) > -1)
            {//delete,update
                if (param == null || param.Count == 0)
                    throw new DappersException("删除、更新语句必须至少提供一个参数！");
            }
        }

        public IEnumerable<T> Select<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value)
        {
            QueryInfo<T> info = new QueryInfo<T>()
               .AddParam(prop, value);
            GetDynamicOrDefaultSql(info);

            return Dao.Query<T>(info);
        }

        public int SelectCount<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value)
        {
            QueryInfo<T> info = new QueryInfo<T>()
               .AddParam(prop, value);
            GetDynamicOrDefaultSql(info);

            return Dao.QueryCount(info);
        }

        public T SelectById<T>(Object id)
        {
            string sql = MappingInfo.GetMappingInfo<T>().SelectById;
            return Dao.Query<T>(sql,
                    new Dictionary<string, object>() { { GetPrimaryKey(typeof(T)), id } }
                ).SingleOrDefault();
        }

        public object SelectById(Type type, Object id)
        {
            QueryInfo info = new QueryInfo(type);
            info.CustomSQL = MappingInfo.GetMappingInfo(type).SelectById;
            info.AddParam(GetPrimaryKey(type), id);
            return Dao.Query(info.ToSQLString(), info.Parameters, type)
                .SingleOrDefault();
        }

        /// <typeparam name="T">为空时，从T创建语句</typeparam>
        public T SelectOne<T>(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            string sql = null;
            if (param is string)
            {
                param = new Dictionary<string, object>() { { GetPrimaryKey(typeof(T)), param } };
                sql = StatementParser.ParseDynamicSql(mapSql, param as Dictionary<string, object>);
            }
            else
                sql = AddParamToSqlWhere(typeof(T), mapSql, ref param);

            return Dao.Query<T>(sql, param).SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapSql">为空时，从T创建语句</param>
        /// <param name="param">/param>
        public IEnumerable<T> Select<T>(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            string sql = AddParamToSqlWhere(typeof(T), mapSql, ref param);

            return Dao.Query<T>(sql, param);
        }

        //public System.Data.DataTable SelectDataTable(string mapSql, object param)
        //{
        //    CheckSqlInject(mapSql);
        //    string sql = AddParamToSqlWhere(typeof(DataTable), mapSql, ref param);

        //    return Dao.QueryDataTable(sql, param);
        //}

        //public System.Data.DataTable SelectDataTable(QueryInfo info)
        //{
        //    GetDynamicOrDefaultSql(info);
        //    return Dao.QueryDataTable(info.ToSQLString(), info.Parameters);
        //}

        public IEnumerable<object> Select(QueryInfo info)
        {
            GetDynamicOrDefaultSql(info);

            return Dao.Query(info);
        }

        public IEnumerable<T> Select<T>(QueryInfo<T> info)
        {
            GetDynamicOrDefaultSql(info);

            return Dao.Query<T>(info);
        }

        public T SelectOne<T>(QueryInfo<T> info)
        {
            return this.Select<T>(info).SingleOrDefault();
        }

        public List<T> SelectList<T>(QueryInfo<T> info)
        {
            return this.Select<T>(info).ToList();
        }

        public QueryInfo<T> SelectInfo<T>(QueryInfo<T> info)
        {
            GetDynamicOrDefaultSql(info);

            Dao.QueryPaginate(info);
            return info;
        }

        public QueryInfo SelectInfo(QueryInfo info)
        {
            GetDynamicOrDefaultSql(info);

            Dao.QueryPaginate(info);
            info.MappingType = null;
            return info;
        }

        public int SelectCount(QueryInfo info)
        {
            GetDynamicOrDefaultSql(info);

            return Dao.QueryCount(info);
        }

        //public IEnumerable<T> Select<T>(string mapSql, System.Linq.Expressions.Expression<Func<T, bool>> where)
        //{
        //    CheckSqlInject(mapSql);
        //    string sql = Dao.MapStatement(mapSql);
        //    if (sql == null) { sql = mapSql = "*"; }
        //    if (sql == mapSql && sql.IndexOf("select ", StringComparison.InvariantCultureIgnoreCase) < 0 && sql.IndexOf(" from", StringComparison.InvariantCultureIgnoreCase) < 0)
        //    {
        //        sql = "select " + mapSql + " from " + QueryInfo.GetMappingInfo<T>().Table + " t0";
        //    }
        //    return Dao.Query<T>(sql,where);
        //}

        public Dappers.GridReader SelectMultiple(QueryInfo info)
        {
            GetDynamicOrDefaultSql(info);

            return Dao.QueryMultiple(info);
        }
        public Dappers.GridReader SelectMultiple(string mapSql, object param)
        {
            CheckSqlInject(mapSql);
            string sql = AddParamToSqlWhere(typeof(Dappers.GridReader), mapSql, ref param);
            return Dao.QueryMultiple(sql, param);
        }

        /// <summary>
        /// 根据T创建select，结合param生成where语句
        /// </summary>
        string AddParamToSqlWhere(Type type, string mapSql, ref object param)
        {
            if (string.IsNullOrEmpty(mapSql))//类型获取
                return GetMappedOrDefaultSql(type);

            if (param != null)
            {
                QueryInfo info = new QueryInfo();//添加当前参数，通过参数名可以控制语句类型：EQ，LK，GT等

                ////参数类型Anonymous对象？ 转换为IDictionary
                //if (!typeof(System.Collections.IDictionary).IsAssignableFrom(param.GetType()))
                //{
                //    var props = Dappers.Context.Reflection.GetHolder(param.GetType());
                //    Dictionary<string, Dappers.Context.GetHandler> getters = props.Getters;

                //    Dictionary<string, object> di = new Dictionary<string, object>(getters.Count);
                //    foreach(var getter in getters){
                //        di.Add(getter.Key,
                //            getter.Value(param) );
                //    }
                //    param = di;
                //}

                info.Parameters = (IDictionary<string, object>)param;
                if (!string.IsNullOrEmpty(mapSql))//重新映射
                    info.CustomSQL = mapSql;
                StatementParser.ParseDynamicSql(info);//配置语句 动态构造
                param = info.Parameters;//参数已被修改

                return info.ToSQLString() + info.ToOrderBy();
            }
            else
            {
                string sql = StatementParser.GetMappedStaticSql(mapSql);
                if (sql.Equals(mapSql) && sql.IndexOf(" ") < 0)
                    throw new ArgumentOutOfRangeException("无效的Sql配置项，请检查Key是否正确：" + mapSql);
                return sql;
            }
        }

        void CheckSqlInject(string sql)
        {
            if (sql != null && sql.IndexOf(' ') > 0)
            {
                if (HasSqlInject(sql))
                    throw new ArgumentException("无效的SQL语句!");
            }
        }

        static Regex regExSql = new Regex("create |alter |truncate |exec"
            , RegexOptions.Compiled | RegexOptions.IgnoreCase);//仅允许select 禁止sql中使用dba_/user_/v$等系统表

        /// <summary>
        /// 仅允许select操作，并禁止对oracle系统表select
        /// </summary>
        /// <param name="sArgs"></param>
        /// <returns></returns>
        static bool HasSqlInject(string sArgs)
        {
            if (sArgs != null && regExSql.IsMatch(sArgs))
                return true;
            return false;
        }

        #region Parse Config Mapped Sql
        private void GetDynamicOrDefaultSql(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                if (string.IsNullOrEmpty(info.CustomSQL))//从Type获取SQL
                {
                    info.CustomSQL = GetMappedOrDefaultSql(info.GetMappingType());
                }
                else//已经制定SQL，动态化！
                {
                    StatementParser.ParseDynamicSql(info);
                }
            }
        }

        ///获取xml中配置的 User.Select语句， 或者根据对象的属性映射生成语句
        private string GetMappedOrDefaultSql(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("MappingType", "QueryInfo未指定查询的对象类型!");

            string key = type.Name + ".Select";
            string sql = StatementParser.GetMappedStaticSql(key);
            if (sql.Equals(key))
                sql = (MappingInfo.GetMappingInfo(type).Select + "t");
            return sql;
        }
        #endregion

    }
}
