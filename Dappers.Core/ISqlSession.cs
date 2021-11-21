using System;
using System.Collections.Generic;

namespace Dappers
{
    /// <summary>
    /// 多数据库实例时，使用此实例
    /// </summary>
    public interface ISqlSession
    {
        IDao Dao { get; }
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
        /// <returns>删除的行数</returns>
        int Delete(Type type, IDictionary<string, object> param);
        bool Delete<T>(Object id);
        bool Delete(Object obj);
        bool Insert(Object obj, string extTableName = null);
        bool Insert(Object obj, out Int32 lastInsertId, string extTableName = null);
        int Insert<T>(IList<T> li, string extTableName = null);
        int Update<T>(IList<T> li, string extTableName = null);
        bool Update(Object obj, string extTableName = null);
        /// <summary>
        /// 根据字典内属性，执行对应局部更新
        /// </summary>
        int Update<T>(System.Collections.IDictionary di, QueryInfo<T> info);
        /// <summary>
        /// 单一属性快捷查找
        /// </summary>
        IEnumerable<T> Select<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value);
        IEnumerable<T> Select<T>(QueryInfo<T> info);
        IEnumerable<object> Select(QueryInfo info);
        /// <summary>
        /// mapSql将检测配置节DB_ADAPTERS对应的key，动态构造并返回新的sql
        /// </summary>
        /// <param name="mapSql">如SysUser.List，动态处理{criteria [condition]}</param>
        /// <param name="param">必须为IDictionary类型才进行动态处理。 若不存在key,则不加载语句!</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(string mapSql, object param);

        int Execute(string mapSql, IDictionary<string, object> param);
        int Execute(QueryInfo info);

        T SelectOne<T>(QueryInfo<T> info);
        T SelectOne<T>(string mapSql, object param);
        T SelectById<T>(Object id);
        object SelectById(Type type, Object id);

        /// <summary>
        /// 将查询后的Reader返回到DataTable
        /// </summary>
        /// <param name="mapSql"></param>
        /// <param name="param"></param>
        /// <returns>System.Data.DataTable</returns>
        //System.Data.DataTable SelectDataTable(string mapSql, object param);
        //System.Data.DataTable SelectDataTable(QueryInfo info);

        /// <summary>
        /// 当TotalCount==-1时 返回指定页数据
        /// </summary>
        List<T> SelectList<T>(QueryInfo<T> info);
        /// <summary>
        /// 当TotalCount==1时 返回总数+分页数据
        /// </summary>
        QueryInfo SelectInfo(QueryInfo info);

        /// <summary>
        /// 当TotalCount==1时 返回总数+分页数据
        /// </summary>
        QueryInfo<T> SelectInfo<T>(QueryInfo<T> info);

        /// <summary>
        /// 自动截取组装成count(*)语句，并统计总数
        /// </summary>
        int SelectCount(QueryInfo info);
        int SelectCount<T>(System.Linq.Expressions.Expression<Func<T, object>> prop, object value);
        ///// <summary>
        ///// 仅输出对象局部字段时，可设mapSql="Name,EMail"
        ///// </summary>
        //IEnumerable<T> Select<T>(string mapSql, System.Linq.Expressions.Expression<Func<T, bool>> where);

        /// <summary>
        /// 此方法可处理SP output参数,Oracle Cursor: info.AddParam("out_mycursor","CURSOR")
        /// 值为null的参数将被忽略,参数out_RETURN 为返回值
        /// </summary>
        Dappers.GridReader SelectMultiple(QueryInfo info);
        /// <summary>
        /// 此方法可处理SP output参数,Oracle Cursor: info.AddParam("out_mycursor","CURSOR")
        /// 值为null的参数将被忽略,参数out_RETURN 为返回值
        /// </summary>
        Dappers.GridReader SelectMultiple(string mapSql, object param);
    }
}
