using AntJoin.Core.Domains;
using AntJoin.Core.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AntJoin.Repository
{
    /// <summary>
    /// 只读仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// 工作单元
        /// </summary>
        protected UnitOfWork UnitOfWork { get; }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="unitOfWork">工作单元</param>
        public ReadOnlyRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = (UnitOfWork)unitOfWork;
        }


        /// <summary>
        /// 获取数据源表
        /// </summary>
        public IQueryable<TEntity> Table => UnitOfWork.Set<TEntity>();


        /// <summary>
        /// 统计数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> Count(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null ?
                await Table.AsNoTracking().CountAsync(cancellationToken) :
                await Table.AsNoTracking().CountAsync(predicate, cancellationToken);
        }


        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return predicate != null ?
                await Table.AsNoTracking().AnyAsync(predicate, cancellationToken) :
                false;
        }


        /// <summary>
        /// 根据条件获取数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null ?
                await Table.ToListAsync(cancellationToken) :
                await Table.Where(predicate).ToListAsync();
        }


        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TEntity> Get(object id, CancellationToken cancellationToken = default)
        {
            return await ((DbSet<TEntity>)Table).FindAsync(new[] { id }, cancellationToken);
        }


        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TEntity> Single(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return predicate != null ?
                await Table.SingleOrDefaultAsync(predicate, cancellationToken) :
                null;

        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteSql(string sql, params object[] parameters)
        {
            return await UnitOfWork.Database.ExecuteSqlRawAsync(sql, parameters);
        }


        /// <summary>
        /// 执行SQL查询语句
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TResult>> FromSql<TResult>(string sql, object parameter) where TResult : class
        {
            return await UnitOfWork.Connection().QueryAsync<TResult>(sql, parameter);
        }
    }



    /// <summary>
    /// 查询仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class ReadOnlyRepository<TEntity, TKey> : ReadOnlyRepository<TEntity>, IReadOnlyRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ReadOnlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
