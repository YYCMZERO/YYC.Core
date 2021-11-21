using AntJoin.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using WL.Think.Dependency;

namespace AntJoin.Repository
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        /// <summary>
        /// HttpContextAccessor访问
        /// </summary>
        private readonly HttpContext _httpContext;


        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="options"></param>
        public UnitOfWork(DbContextOptions options) : base(options)
        {
            _httpContext = ServiceLocator.Instance.HttpContext();
        }




        /// <summary>
        /// 配置映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new EntityMapper().Map(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }



        #region 事务操作
        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> Commit(CancellationToken cancellationToken = default)
        {
            return await SaveChangesAsync(cancellationToken);
        }
        #endregion



        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public IDbConnection Connection() => Database?.GetDbConnection();


    }
}
