using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using NLog;
using System.Threading.Tasks;

namespace AntJoin.Dapper.Query
{
    public partial class Dao : IDao
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public Dao(string connStr, string connDriverTypeName)
        {
            var m = new ConnectionManager
            {
                ConnectionString = connStr,
                ConnectionTypeName = connDriverTypeName
            };
            ConnectionManager = m;
        }


        public Dao(ConnectionManager cm)
        {
            ConnectionManager = cm;
        }

        public ConnectionManager ConnectionManager { get; }
        private IDbConnection Conn => ConnectionManager.GetConnection();
        private IDbTransaction Trans => ConnectionManager.GetTransaction();
        public string Database => Conn.Database;


        public IDbTransaction TxBegin()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("TxBegin - " + GetCallingStackTrace());
            }
            return ConnectionManager.TxBegin(IsolationLevel.Unspecified);
        }

        public void TxCommit() => ConnectionManager.TxCommit();


        /// <summary>
        ///Tx之中若调用SP，则SP内部禁用commit， 否则导致Rollback无效
        /// </summary>
        public void TxRollback() => ConnectionManager.TxRollback();


        public IDbTransaction TxBegin(IsolationLevel il) => ConnectionManager.TxBegin(il);

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            ConnectionManager.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbConnection Open()
        {
            return ConnectionManager.Open();
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string BuildPagingSql(QueryInfo info)
        {
            #region re-calculate limition
            var iCount = info.TotalCount;
            var iStartRecord = info.StartRecord;
            var iPageSize = info.PageSize;

            if (iCount > -1)
            {
                //该函数会导致如果要取的数据超过总数，仍然会查询到数据，而不是想要的空数组
                //startRecord excceed maxRecord,reset
                //if (iCount <= iStartRecord)
                //{
                //    iStartRecord = iCount / iPageSize;//2.3=>2 取整
                //    iStartRecord *= iPageSize;
                //    info.StartRecord = iStartRecord;//Start Index changed.
                //}

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
                    return $"{info.ToSQLString()} {info.ToOrderBy()} LIMIT {iStartRecord},{iPageSize}";
                }
                else if (ConnectionManager.ParamPrefix == ":")//ORACLE
                {
                    return string.Format(
                        @"SELECT r2.* FROM ( SELECT rownum rn,r1.* FROM ( {2} ) r1 WHERE rownum <= {0} ) r2 WHERE r2.rn > {1}"
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



        #region 异步方法
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, object param)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(GetCallingStackTrace() + sql + LogParam(param));
            }
            try
            {
                return await Conn.ExecuteAsync(ChangePrefix(sql), param
                    , Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }


        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                return await ExecuteAsync(info.ToSQLString(), info.Parameters);
            }
            else
            {
                return await Conn.ExecuteAsync(info.NamedQuery, info.Parameters, Trans, null, CommandType.StoredProcedure);
            }
        }


        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object param)
        {
            return await Conn.ExecuteScalarAsync<T>(ChangePrefix(sql), param
                , Trans, null, null);
        }


        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<int> QueryCountAsync(QueryInfo info)
        {
            var sql = info.ToSQLString();
            if (sql.IndexOf(" count(", StringComparison.CurrentCultureIgnoreCase) < 0)
            {
                var i = sql.IndexOf(" from ", StringComparison.CurrentCultureIgnoreCase);
                if (i > -1)
                {
                    sql = $"select count({info.CountField}) {sql.Substring(i)}";//from ...
                }
            }

            if (Log.IsDebugEnabled)
            {
                Log.Debug("select count(*) - " + GetCallingStackTrace() + sql);
            }

            object o = null;
            try
            {
                return await ExecuteScalarAsync<int>(sql, info.Parameters);
            }
            catch (Exception ex)
            {
                throw new DappersException($"{ex.Message} \r\n #{o} <= {sql} {LogParam(info.Parameters)}", ex);
            }
        }


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(GetCallingStackTrace() + sql + LogParam(param));
            }
            try
            {
                return await Conn.QueryAsync<T>(ChangePrefix(sql), param
                    , Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                string sql;
                if (info.TotalCount == 1 || info.TotalCount == -1)//only paging
                {
                    if (info.TotalCount == 1) //fetch COUNT !
                    {
                        info.TotalCount = await QueryCountAsync(info);
                    }

                    sql = BuildPagingSql(info);
                }
                else
                {
                    sql = info.ToSQLString() + info.ToOrderBy();
                }
                return await QueryAsync<T>(sql, info.Parameters);
            }
            else
            {
                return await Conn.QueryAsync<T>(info.NamedQuery, info.Parameters
                    , Trans, null, CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IEnumerable<object>> QueryAsync(string sql, object param, Type type)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(type.Name + " - " + GetCallingStackTrace() + sql + LogParam(param));
            }
            try
            {
                return await Conn.QueryAsync(type, sql, param, Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IEnumerable<object>> QueryAsync(QueryInfo info)
        {
            var type = info.MappingType as Type;
            if (type == null && info.MappingType is string mappingType)
            {
                type = Type.GetType(mappingType, true);
            }

            if (type == null)
            {
                type = typeof(object);
            }

            if (info.NamedQuery == null)
            {
                string sql;
                if (info.TotalCount == 1 || info.TotalCount == -1)//only paging
                {
                    if (info.TotalCount == 1) //fetch COUNT !
                    {
                        info.TotalCount = await QueryCountAsync(info);
                    }

                    sql = BuildPagingSql(info);
                }
                else
                {
                    sql = info.ToSQLString() + info.ToOrderBy();
                }
                return await QueryAsync(sql, info.Parameters, type);
            }
            else
            {
                return await Conn.QueryAsync(info.NamedQuery, info.Parameters
                    , Trans, null, CommandType.StoredProcedure);
            }
        }


        /// <summary>
        /// 查询分页数据， TotalCount=1时，执行 分页查询+总数
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<QueryInfo> QueryPaginateAsync(QueryInfo info)
        {
            info.List = (await QueryAsync(info)).ToList();
            if (ConnectionManager.GetTransaction() == null) //主动关闭连接
            {
                Close();
            }
            return info;
        }


        /// <summary>
        /// 查询分页数据， TotalCount=1时，执行 分页查询+总数
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<QueryInfo<T>> QueryPaginateAsync<T>(QueryInfo<T> info)
        {
            info.List = (await QueryAsync<T>(info)).ToList();
            if (ConnectionManager.GetTransaction() == null) //主动关闭连接
            {
                Close();
            }
            return info;
        }


        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> QueryFirstAsync<T>(string sql, object param)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(GetCallingStackTrace() + sql + LogParam(param));
            }
            try
            {
                return await Conn.QueryFirstAsync<T>(ChangePrefix(sql), param
                    , Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> QueryFirstAsync<T>(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                var sql = info.ToSQLString() + info.ToOrderBy();
                return await QueryFirstAsync<T>(sql, info.Parameters);
            }
            else
            {
                return await Conn.QueryFirstAsync<T>(info.NamedQuery, info.Parameters
                    , Trans, null, CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(GetCallingStackTrace() + sql + LogParam(param));
            }
            try
            {
                return await Conn.QueryFirstOrDefaultAsync<T>(ChangePrefix(sql), param
                    , Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }


        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                var sql = info.ToSQLString() + info.ToOrderBy();
                return await QueryFirstOrDefaultAsync<T>(sql, info.Parameters);
            }
            else
            {
                return await Conn.QueryFirstOrDefaultAsync<T>(info.NamedQuery, info.Parameters
                    , Trans, null, CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> QuerySingleAsync<T>(string sql, object param)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(GetCallingStackTrace() + sql + LogParam(param));
            }
            try
            {
                return await Conn.QuerySingleAsync<T>(ChangePrefix(sql), param
                    , Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }



        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> QuerySingleAsync<T>(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                var sql = info.ToSQLString() + info.ToOrderBy();
                return await QuerySingleAsync<T>(sql, info.Parameters);
            }
            else
            {
                return await Conn.QuerySingleAsync<T>(info.NamedQuery, info.Parameters
                    , Trans, null, CommandType.StoredProcedure);
            }
        }



        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(GetCallingStackTrace() + sql + LogParam(param));
            }
            try
            {
                return await Conn.QuerySingleOrDefaultAsync<T>(ChangePrefix(sql), param
                    , Trans, null, null);
            }
            catch (Exception ex)
            {
                throw new DappersException(ex.Message + "\r\n" + sql + LogParam(param), ex);
            }
        }


        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<T> QuerySingleOrDefaultAsync<T>(QueryInfo info)
        {
            if (info.NamedQuery == null)
            {
                var sql = info.ToSQLString() + info.ToOrderBy();
                return await QuerySingleOrDefaultAsync<T>(sql, info.Parameters);
            }
            else
            {
                return await Conn.QuerySingleOrDefaultAsync<T>(info.NamedQuery, info.Parameters
                    , Trans, null, CommandType.StoredProcedure);
            }
        }
        #endregion


        #region 辅助方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private string LogParam(object param)
        {
            switch (param)
            {
                case null:
                case string _:
                    return (string)param;
                case IDictionary dictionary:
                {
                    var sb = new System.Text.StringBuilder();
                    var enumer = dictionary.GetEnumerator();
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
                default:
                    return "\r\n\tObject Param:" + param.GetType().Name + " Id=" + param;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetCallingStackTrace()
        {
            return string.Empty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string ChangePrefix(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
            return BChangePrefix ? 
                Regex.Replace(sql, StatementParser.PREFIX, ConnectionManager.ParamPrefix, RegexOptions.Multiline) : 
                sql;
        }


        /// <summary>
        /// 
        /// </summary>
        private bool BChangePrefix => ConnectionManager.ParamPrefix != StatementParser.PREFIX;
        

        #endregion
        

        public void Dispose()
        {
            ConnectionManager.Dispose();
        }

        ~Dao()
        {
            Dispose();
        }
    }
}
