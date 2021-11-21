using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using Dappers.Query;
using NLog;
using System.Diagnostics;

namespace Dappers.Query
{
    public partial class Dao : IDao
    {
        protected static Logger log = LogManager.GetCurrentClassLogger();
        public Dao(string connStr, string connDriverTypeName)
        {
            ConnectionManager m = new ConnectionManager();
            m.ConnectionString = connStr;
            m.ConnectionTypeName = connDriverTypeName;
            this.cm = m;
        }
        public Dao(ConnectionManager cm)
        {
            this.cm = cm;
        }

        ConnectionManager cm;
        public ConnectionManager ConnectionManager
        {
            get
            {
                return cm;
            }
        }
        private IDbConnection Conn { get { return ConnectionManager.GetConnection(); } }
        private IDbTransaction Trans { get { return ConnectionManager.GetTransaction(); } }
        public IDbTransaction TxBegin()
        {
            if (log.IsDebugEnabled)
                log.Debug("TxBegin - " + getCallingStackTrace());
            return ConnectionManager.TxBegin(IsolationLevel.Unspecified);
        }
        public void TxCommit()
        {
            ConnectionManager.TxCommit();
        }
        /// <summary>
        ///Tx之中若调用SP，则SP内部禁用commit， 否则导致Rollback无效
        /// </summary>
        public void TxRollback()
        {
            ConnectionManager.TxRollback();
        }
        public IDbTransaction TxBegin(IsolationLevel il) { return ConnectionManager.TxBegin(il); }
        public void Close()
        {
            ConnectionManager.Close();
        }
        public IDbConnection Open()
        {
            return ConnectionManager.Open();
        }

        string LogParam(object param)
        {
            if (param == null || param is string)
                return (string)param;
            else if (param is IDictionary)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                IDictionaryEnumerator enumer = ((IDictionary)param).GetEnumerator();
                while (enumer.MoveNext())
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("\t");
                    sb.Append(enumer.Key);
                    sb.Append(StatementParser.PREFIX);
                    sb.Append(enumer.Value);
                }
                return sb.ToString();
            }
            return "\r\n\tObject Param:" + param.GetType().Name + " Id=" + param.ToString();
        }
        /// <param name="sql"></param>
        /// <param name="param">IEnumerable for multiExec</param>
        public int Execute(string sql, object param)
        {
            if (log.IsInfoEnabled)
                log.Info(getCallingStackTrace() + sql + LogParam(param));
            try
            {
                return Conn.Execute(ChangePrefix(sql), param
                    , Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }

        public object ExecuteScalar(string sql, object param)
        {
            return Conn.ExecuteScalar(ChangePrefix(sql), param
                , Trans, null, null);
        }

        public IEnumerable<T> Query<T>(string sql, object param)
        {
            if (log.IsInfoEnabled)
                log.Info(getCallingStackTrace() + sql + LogParam(param));
            try
            {
                return Conn.Query<T>(ChangePrefix(sql), param
                    , Trans, false, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }

        public DataTable QueryDataTable(string sql, object param)
        {
            return null;
        }

        /// <summary>
        /// multiple resultset
        /// </summary>
        public GridReader QueryMultiple(string sql, object param)
        {
            CommandType? cmdType = null;
            if (sql.IndexOf(" from ", StringComparison.CurrentCultureIgnoreCase) < 0)
                cmdType = CommandType.StoredProcedure;

            if (log.IsInfoEnabled)
                log.Info(getCallingStackTrace() + sql + LogParam(param));

            try
            {
                var ps = SqlMapper.ParepareSPParam(Conn, param as IDictionary<string, object>);
                SqlMapper.GridReader reader = Conn.QueryMultiple(ChangePrefix(sql), ps
                    , Trans, null, cmdType);
                return new GridReader(reader, ps);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }


        public int Execute(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                return this.Execute(info.ToSQLString(), info.Parameters);
            }
            else
            {
                return Conn.Execute(info.NamedQuery, info.Parameters, Trans, null, CommandType.StoredProcedure);
            }
        }

        public IEnumerable<T> Query<T>(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                string sql;
                if (info.TotalCount == 1 || info.TotalCount == -1)//only paging
                {
                    if (info.TotalCount == 1)//fetch COUNT !
                        info.TotalCount = this.QueryCount(info);

                    sql = BuildPagingSQL(info);
                }
                else
                    sql = info.ToSQLString() + info.ToOrderBy();
                return this.Query<T>(sql, info.Parameters);
            }
            else
            {
                return Conn.Query<T>(info.NamedQuery, info.Parameters
                    , Trans, false, null, CommandType.StoredProcedure);
            }
        }

        public GridReader QueryMultiple(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                return this.QueryMultiple(info.ToSQLString() + info.ToOrderBy(), info.Parameters);
            }
            else
            {
                return this.QueryMultiple(info.NamedQuery, info.Parameters);
            }
        }

        public int QueryCount(QueryInfo info)
        {
            string sql = info.ToSQLString();
            if (sql.IndexOf(" count(", StringComparison.CurrentCultureIgnoreCase) < 0)
            {
                int i = sql.IndexOf(" from ", StringComparison.CurrentCultureIgnoreCase);
                if (i > -1)
                    sql = string.Format("select count({0}) {1}", info.CountField
                        , sql.Substring(i));//from ...
            }
            if (log.IsDebugEnabled)
                log.Debug("select count(*) - " + getCallingStackTrace() + sql);

            object o = null;
            try
            {
                o = this.ExecuteScalar(sql, info.Parameters);
                if (o == null) o = string.Empty;
                return int.Parse(o.ToString());
            }
            catch (Exception ex)
            {
                throw new DappersException(string.Format("{0} \r\n #{1} <= {2} {3}", ex.Message, o, sql, LogParam(info.Parameters)), ex);
            }
        }
        /*
        public List<object> QueryList(QueryInfo info)
        {
            Type type = info.MappingType as Type;
            if (type == null)
            {
                string t = info.MappingType as string;
                if (t != null)
                    type = Type.GetType(t, true);
            }
            if (info.NamedQuery == null)
            {
                string sql;
                if (info.TotalCount==1 || info.TotalCount == -1)//only paging
                    sql = BuildPagingSQL(info);
                else
                    sql = info.ToSQLString() + info.ToOrderBy();

                if (type == null)
                    return this.QueryByDynamic(ChangePrefix(sql), info.Parameters
                    , Trans, null, null);
                else
                    return this.QueryByType(ChangePrefix(sql), info.Parameters, type
                            , Trans, null, null);
            }
            else
            {
                if (type == null)
                    return this.QueryByDynamic(info.NamedQuery, info.Parameters
                    , Trans, null, CommandType.StoredProcedure);
                else
                    return this.QueryByType(info.NamedQuery, info.Parameters, type
                        , Trans, null, CommandType.StoredProcedure);
            }
        }

        private List<dynamic> QueryByDynamic(string sql, object param,IDbTransaction trans, int? timeout, CommandType? cmdType)
        {
            //Dappers.Cfg.LicenseValidator.Validate();
            if (log.IsDebugEnabled)
                log.Debug(getCallingStackTrace()+sql+LogParam(param));
            try
            {
                return Conn.Query(sql, param, trans, false, timeout, cmdType).ToList();
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message+"\r\n"+getCallingStackTrace()+sql+LogParam(param), ex);
            }
        }*/
        public QueryInfo<T> QueryPaginate<T>(QueryInfo<T> info)
        {
            if (info.TotalCount == 1)//count & paging 
                this.Conn.Open();//为2步执行打开连接

            info.List = this.Query<T>(info).ToList();

            if (ConnectionManager.GetTransaction() == null)//主动关闭连接
                this.Close();
            return info;
        }
        public QueryInfo QueryPaginate(QueryInfo info)
        {
            if (info.TotalCount == 1)//count & paging 
                this.Conn.Open();//为2步执行打开连接

            info.List = this.Query(info).ToList();

            if (ConnectionManager.GetTransaction() == null)//主动关闭连接
                this.Close();
            return info;
        }

        public IEnumerable<object> Query(QueryInfo info)
        {
            Type type = info.MappingType as Type;
            if (type == null && info.MappingType is string)
                type = Type.GetType((string)info.MappingType, true);
            if (type == null) type = typeof(object);

            if (info.NamedQuery == null)
            {
                string sql;
                if (info.TotalCount == 1 || info.TotalCount == -1)//only paging
                {
                    if (info.TotalCount == 1)//fetch COUNT !
                        info.TotalCount = this.QueryCount(info);

                    sql = BuildPagingSQL(info);
                }
                else
                    sql = info.ToSQLString() + info.ToOrderBy();
                return this.Query(sql, info.Parameters, type);
            }
            else
            {
                return Conn.Query(info.NamedQuery, info.Parameters
                    , Trans, false, null, CommandType.StoredProcedure);
            }
        }
        public IEnumerable<object> Query(string sql, object param, Type type)
        {
            if (log.IsDebugEnabled)
                log.Debug(type.Name + " - " + getCallingStackTrace() + sql + LogParam(param));
            try
            {
                return Conn.Query(type, sql, param, Trans, false, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }

        private static string getCallingStackTrace()
        {
            return string.Empty;
        }


        public string ChangePrefix(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            if (bChangePrefix)
                return System.Text.RegularExpressions.Regex.Replace(sql, StatementParser.PREFIX, ConnectionManager.ParamPrefix, System.Text.RegularExpressions.RegexOptions.Multiline);
            return sql;
        }
        bool bChangePrefix
        {
            get
            {
                return ConnectionManager.ParamPrefix != StatementParser.PREFIX;
            }
        }

        public string BuildPagingSQL(QueryInfo info)
        {
            #region re-calculate limition
            int iCount = info.TotalCount;
            int iStartRecord = info.StartRecord;
            int iPageSize = info.PageSize;

            if (iCount > -1)
            {
                //startRecord excceed maxRecord,reset
                if (iCount <= iStartRecord)
                {
                    iStartRecord = iCount / iPageSize;//2.3=>2 取整
                    iStartRecord = iStartRecord * iPageSize;
                    info.StartRecord = iStartRecord;//Start Index changed.
                }

                //Should Reset Last Page's PageSize?
                //存在仅Take数据， 未预先查询count值
                //int iLasPageSize = iCount - iStartRecord;
                //if (iLasPageSize > 0 && iLasPageSize < iPageSize)
                //    iPageSize = iLasPageSize;
            }
            #endregion
            if (ConnectionManager.ParamPrefix != "?")
            {
                if (ConnectionManager.ConnectionTypeName.StartsWith("MySql."))
                {
                    return string.Format("{0} {1} LIMIT {2},{3}",
                        info.ToSQLString(),
                        info.ToOrderBy(),
                        iStartRecord, iPageSize);
                }
                else if (ConnectionManager.ParamPrefix == ":")//ORACLE
                {
                    return string.Format("SELECT r2.* FROM ( SELECT rownum rn,r1.* FROM ( {2} ) r1 WHERE rownum <= {0} ) r2 WHERE r2.rn > {1}"
                        , iStartRecord + iPageSize, iStartRecord, info.ToSQLString() + info.ToOrderBy());
                }
                else//SQL Server?
                {
                    return string.Format(@"SELECT r2.* FROM ( SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS rn,{0} ) r2 WHERE r2.rn BETWEEN {2} AND {3}"
                        , info.ToSQLString().Substring(6)//trim select
                        , string.Join(",", info.OrderBys.ToArray())//then get order by
                        , iStartRecord + 1, iStartRecord + iPageSize);

                    #region Trim all dots and Reverse Order
                    //if (info.OrderBy.Count == 0)
                    //    throw new Exception("'order by' must be supplied for sql server 2000 paging.");
                    ////A.USER_CDE DESC => USER_CDE ASC
                    //System.Text.StringBuilder sbOriginalOrder = new System.Text.StringBuilder();
                    //System.Text.StringBuilder sbReversedOrder = new System.Text.StringBuilder();
                    //for (int i = 0; i < info.OrderBy.Count; i++)
                    //{
                    //    string ord = info.OrderBy[i];
                    //    if (ord.IndexOf(".") > 0)
                    //        ord = ord.Substring(ord.IndexOf(".") + 1);//trim table alias

                    //    sbOriginalOrder.Append(ord);
                    //    if (ord.IndexOf(" DESC") > -1)
                    //        sbReversedOrder.Append(ord.Replace(" DESC", " ASC"));
                    //    else if (ord.IndexOf(" ASC") > -1)
                    //        sbReversedOrder.Append(ord.Replace(" ASC", " DESC"));
                    //    else
                    //        sbReversedOrder.Append(ord + " DESC");

                    //    sbOriginalOrder.Append(",");
                    //    sbReversedOrder.Append(",");
                    //}
                    //sbOriginalOrder.Remove(sbOriginalOrder.Length - 1, 1);
                    //sbReversedOrder.Remove(sbReversedOrder.Length - 1, 1);//trim last ','
                    #endregion
                    //return string.Format("SELECT r2.* FROM(SELECT TOP {0} r1.* FROM(SELECT TOP {1} {2}) r1 ORDER BY {3})r2 ORDER BY {4}"
                    //, iPageSize, iStartRecord + iPageSize, info.ToSQLString().Substring(6) + info.ToOrderBy(), sbReversedOrder, sbOriginalOrder);//Substring(6):trim 'SELECT' in QueryObject
                }
            }
            return info.ToSQLString() + info.ToOrderBy();
        }


    }
}
