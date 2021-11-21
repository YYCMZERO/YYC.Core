using System;
using System.Linq;
using System.Linq.Expressions;

namespace AntJoin.Repository
{
    /// <summary>
    /// <see cref="IQueryable{T}"/>扩展
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="condition">该值为true时添加查询条件，否则忽略</param>
        public static IQueryable<TEntity> WhereIf<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate, bool condition) where TEntity : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return condition ? source.Where(predicate) : source;
        }
    }
}
