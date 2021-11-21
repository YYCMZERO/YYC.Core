using AntJoin.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AntJoin.Core.Repositories
{
    /// <summary>
    /// 只读仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IReadOnlyRepository<TEntity> : IRepository
        where TEntity : class, IEntity
    {
        /// <summary>
        /// 获取数据源表
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// 查找实体列表
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);


        /// <summary>
        /// 查找单个实体
        /// </summary>
        /// <param name="id">标识</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TEntity> Get(object id, CancellationToken cancellationToken = default);


        /// <summary>
        /// 统计查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<int> Count(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);


        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);


        /// <summary>
        /// 查找单个实体，会追踪
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TEntity> Single(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }


    /// <summary>
    /// 只读仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>
        where TEntity : class, IEntity<TKey>
    {
    }
}
