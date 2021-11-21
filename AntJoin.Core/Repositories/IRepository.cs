using AntJoin.Core.Domains;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AntJoin.Core.Repositories
{
    /// <summary>
    /// 仓储基类定义
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 执行SQL语句操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> ExecuteSql(string sql, params object[] parameters);


        /// <summary>
        /// 执行SQL语句查询操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> FromSql<TResult>(string sql, object parameter = null) where TResult : class;
    }


    /// <summary>
    /// 仓储定义
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task Add(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task Add(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// 批量添加，添加会自动提交
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task BulkAdd(IEnumerable<TEntity> entities, IDbTransaction trans = null);



        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">自动保存</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// 修改实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="autoSave">自动保存</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// 异步更新所有符合特定条件的实体
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="updateExpression">实体更新表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task Update(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression, CancellationToken cancellationToken = default);


        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="id">标识</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Remove(object id, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Remove(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// 移除实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Remove(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);


        /// <summary>
        /// 异步删除所有符合特定条件的实体
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task Remove(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }


    /// <summary>
    /// 仓储定义
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {

    }
}
