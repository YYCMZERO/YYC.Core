using AntJoin.Core.Domains;
using AntJoin.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AntJoin.Repository
{
    /// <summary>
    /// 基础仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="unitOfWork"></param>
        public Repository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _dbSet = Table as DbSet<TEntity>;
        }

        /// <summary>
        /// 获取工作单元
        /// </summary>
        /// <returns></returns>
        public UnitOfWork GetUnitOfWork() => UnitOfWork;

        #region 新增操作
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave">自动保存</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Add(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entity != null)
            {
                await _dbSet.AddAsync(entity, cancellationToken);
                await AutoSave(autoSave, cancellationToken);
            }
        }


        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Add(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entities != null || entities.Any())
            {
                await _dbSet.AddRangeAsync(entities, cancellationToken);
                await AutoSave(autoSave, cancellationToken);
            }
        }


        /// <summary>
        /// bulk 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public virtual async Task BulkAdd(IEnumerable<TEntity> entities, IDbTransaction trans = null)
        {
            await Add(entities, true);
        }
        #endregion


        #region 删除操作
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Remove(object id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var item = await Get(id, cancellationToken);
            if (item != null)
            {
                await Remove(item, autoSave, cancellationToken);
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Remove(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await AutoSave(autoSave, cancellationToken);
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Remove(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entities != null && entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await AutoSave(autoSave, cancellationToken);
            }
        }



        /// <summary>
        /// 删除，直接更新到数据库
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Remove(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                await _dbSet.Where(predicate).DeleteFromQueryAsync(cancellationToken);
            }
        }
        #endregion


        #region 更新操作
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Update(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entity != null)
            {
                //_dbSet.Attach(entity);
                _dbSet.Update(entity);
                await AutoSave(autoSave, cancellationToken);
            }
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Update(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (entities != null && entities.Any())
            {
                _dbSet.UpdateRange(entities);
                await AutoSave(autoSave, cancellationToken);
            }
        }


        /// <summary>
        /// 更新，直接更新到数据库
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task Update(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression, CancellationToken cancellationToken = default)
        {
            if (predicate != null && updateExpression != null)
            {
                await _dbSet.Where(predicate).UpdateFromQueryAsync(updateExpression, cancellationToken);
            }
        }
        #endregion



        #region 自动提交
        /// <summary>
        /// 自动提交
        /// </summary>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task AutoSave(bool autoSave = false, CancellationToken cancellationToken = default)
        {
            if (autoSave)
            {
                await UnitOfWork.Commit(cancellationToken);
            }
        }
        #endregion
    }


    /// <summary>
    /// 基础仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class Repository<TEntity, TKey> : Repository<TEntity>, IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="unitOfWork"></param>
        public Repository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
