using System;
using System.Collections.Generic;
using System.Data;
using AntJoin.Dapper.Query;
using System.Threading.Tasks;

namespace AntJoin.Dapper
{
    public interface IDao : IDisposable
    {
        ConnectionManager ConnectionManager { get; }
        string Database { get; }
        IDbTransaction TxBegin();
        IDbTransaction TxBegin(IsolationLevel il);
        void TxCommit();
        void TxRollback();
        IDbConnection Open();
        
        /// <summary>
        /// close connection and commit transaction
        /// </summary>
        void Close();

        //------------------QUERY / EXECUTE----------------------------------
        #region 异步方法
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param);

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(QueryInfo info);


        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string sql, object param);


        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<int> QueryCountAsync(QueryInfo info);


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param);


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(QueryInfo info);


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<IEnumerable<object>> QueryAsync(string sql, object param, Type type);


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IEnumerable<object>> QueryAsync(QueryInfo info);

        /// <summary>
        /// 查询分页数据， TotalCount=1时，执行 分页查询+总数
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<QueryInfo> QueryPaginateAsync(QueryInfo info);

        /// <summary>
        /// 查询分页数据， TotalCount=1时，执行 分页查询+总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<QueryInfo<T>> QueryPaginateAsync<T>(QueryInfo<T> info);



        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> QueryFirstAsync<T>(string sql, object param);

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> QueryFirstAsync<T>(QueryInfo info);

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param);


        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(QueryInfo info);

        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> QuerySingleAsync<T>(string sql, object param);

        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> QuerySingleAsync<T>(QueryInfo info);

        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param);


        /// <summary>
        /// 查询唯一一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> QuerySingleOrDefaultAsync<T>(QueryInfo info);

        #endregion
    }
}
