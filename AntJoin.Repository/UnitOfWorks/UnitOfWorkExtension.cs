using AntJoin.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AntJoin.Repository
{
    /// <summary>
    /// EF工作单元扩展方法
    /// </summary>
    public static class UnitOfWorkExtension
    {
        /// <summary>
        /// 情况Entity实体追踪缓存
        /// </summary>
        /// <param name="unitOfWork"></param>
        public static void Clear(this IUnitOfWork unitOfWork)
        {
            if (unitOfWork is DbContext dbContext)
            {
                dbContext.ChangeTracker.Entries()
                    .ToList()
                    .ForEach(s => s.State = EntityState.Detached);
            }
        }
    }
}
