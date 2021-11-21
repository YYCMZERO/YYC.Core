using AntJoin.Core.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AntJoin.Repository
{
    /// <summary>
    /// 分页扩展
    /// </summary>
    public static class PagingExtension
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="total">总共行数</param>
        /// <param name="page">第几页</param>
        /// <param name="size">每页行数</param>
        /// <returns></returns>
        public static async Task<PagingResult<TEntity>> ToPage<TEntity>(this IOrderedQueryable<TEntity> source, int total, int page = 1, int size = 20)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var rows = await source.Page(page, size).ToListAsync();
            return new PagingResult<TEntity>(total, rows);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="page">第几页</param>
        /// <param name="size">每页行数</param>
        /// <param name="orderby">排序</param>
        /// <param name="total">总共行数</param>
        /// <returns></returns>
        public static async Task<PagingResult<TEntity>> ToPage<TEntity>(this IQueryable<TEntity> source, string orderby, int page = 1, int size = 20, int total = 0)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (string.IsNullOrWhiteSpace(orderby))
            {
                throw new ArgumentNullException(nameof(orderby), "没有配置排序条件");
            }
            if (total == 0)
            {
                total = await source.CountAsync();
            }
            var rows = await source.OrderBy(orderby).Page(page, size).ToListAsync();
            return new PagingResult<TEntity>(total, rows);
        }
    }
}
