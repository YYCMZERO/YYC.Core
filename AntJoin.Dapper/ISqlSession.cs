using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Dapper
{
    /// <summary>
    /// 多数据库实例时，使用此实例
    /// </summary>
    public interface ISqlSession : IDisposable
    {
        //IDao Dao { get; }
        void TxBegin();
        void TxRollback();
        void TxCommit();
        /// <summary>
        /// Close connection and commit transaction
        /// </summary>
        void Close();
        

        /// <summary>
        /// 动态构造语句进行批量删除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(Type type, IDictionary<string, object> param);

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync<T>(object id);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(object obj);

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="extTableName">扩展表名称</param>
        /// <returns></returns>
        Task<bool> InsertAsync(object obj, string extTableName = null);
        

        /// <summary>
        /// 插入数据，返回ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="extTableName"></param>
        /// <returns></returns>
        Task<T> InsertAsync<T>(object obj, string extTableName = null);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="li"></param>
        /// <param name="extTableName"></param>
        /// <returns></returns>
        Task<int> InsertAsync<T>(IList<T> li, string extTableName = null);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="extTableName"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(object obj, string extTableName = null);

        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="li"></param>
        /// <param name="extTableName"></param>
        /// <returns>受影响行数</returns>
        Task<int> UpdateAsync<T>(IList<T> li, string extTableName = null);

        /// <summary>
        /// 根据字典内属性，执行对应局部更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="di"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(System.Collections.IDictionary di, QueryInfo<T> info);

        /// <summary>
        /// 执行sql（注：有加入缓存读写的表格不可以使用该方法）
        /// </summary>
        /// <param name="mapSql"></param>
        /// <param name="param">支持Dictionary类型</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string mapSql, IDictionary<string, object> param);

        /// <summary>
        /// 执行sql（注：有加入缓存读写的表格不可以使用该方法）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(QueryInfo info);

        /// <summary>
        /// 单一属性快速查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectAsync<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value);

        
        /// <summary>
        /// mapSql将检测配置节DB_ADAPTERS对应的key，动态构造并返回新的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql">如SysUser.List，动态处理{criteria [condition]}</param>
        /// <param name="param">必须为IDictionary类型才进行动态处理。 若不存在key,则不加载语句!</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectAsync<T>(string mapSql, object param);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectAsync<T>(QueryInfo<T> info);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IEnumerable<object>> SelectAsync(QueryInfo info);
        

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> SelectByIdAsync<T>(object id);

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<object> SelectByIdAsync(Type type, object id);

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> SelectSingleOrDefaultAsync<T>(string mapSql, object param);
        
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> SelectSingleOrDefaultAsync<T>(QueryInfo<T> info);

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> SelectSingleAsync<T>(string mapSql, object param);
        
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> SelectSingleAsync<T>(QueryInfo<T> info);


        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> SelectFirstOrDefaultAsync<T>(string mapSql, object param);
        

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> SelectFirstOrDefaultAsync<T>(QueryInfo<T> info);


        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> SelectFirstAsync<T>(string mapSql, object param);


        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<T> SelectFirstAsync<T>(QueryInfo<T> info);
        

        /// <summary>
        /// 自动截取组装成count(*)语句，并统计总数
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<int> SelectCountAsync(QueryInfo info);


        /// <summary>
        /// 统计总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<int> SelectCountAsync<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value);
        

        /// <summary>
        /// 返回指定页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<List<T>> SelectListAsync<T>(QueryInfo<T> info);
        

        /// <summary>
        /// 返回总数+分页数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<QueryInfo> SelectInfoAsync(QueryInfo info);


        /// <summary>
        /// 返回总数+分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<QueryInfo<T>> SelectInfoAsync<T>(QueryInfo<T> info);


        /// <summary>
        /// 返回数据+是否有下一页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<QueryInfo<T>> SelectInfoNextPageAsync<T>(QueryInfo<T> info);
    }
}
