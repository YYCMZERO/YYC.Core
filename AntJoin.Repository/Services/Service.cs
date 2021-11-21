using AntJoin.Core.Domains;
using AntJoin.Core.Repositories;
using AntJoin.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using WL.Think.Dependency;

namespace AntJoin.Repository
{
    /// <summary>
    /// 服务实现
    /// </summary>
    [IgnoreDependency]
    public abstract class Service : IService
    {
        /// <summary>
        /// 工作单元
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }


        /// <summary>
        /// 请求上下文
        /// </summary>
        protected HttpContext HttpContext { get; }


        /// <summary>
        /// 日志
        /// </summary>
        protected ILogger Logger { get; }


        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="unitOfWork"></param>
        public Service(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            HttpContext = ServiceLocator.Instance.HttpContext();
            Logger = ServiceLocator.Instance.GetService<ILoggerFactory>()?.CreateLogger(GetType());
        }


        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public async Task<int> Commit()
        {
            return await UnitOfWork.Commit();
        }
    }



    /// <summary>
    /// 服务实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [IgnoreDependency]
    public class Service<TEntity> : Service, IService<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// 实体对应仓储
        /// </summary>
        protected IRepository<TEntity> Repository { get; }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="repository"></param>
        public Service(IUnitOfWork unitOfWork, IRepository<TEntity> repository) : base(unitOfWork)
        {
            Repository = repository;
        }

        /// <summary>
        /// 查询
        /// </summary>
        public IQueryable<TEntity> Query => (UnitOfWork as UnitOfWork).Set<TEntity>();
    }



    /// <summary>
    /// 服务实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    [IgnoreDependency]
    public class Service<TEntity, TKey> : Service<TEntity>, IService<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="repository"></param>
        public Service(IUnitOfWork unitOfWork, IRepository<TEntity, TKey> repository) : base(unitOfWork, repository)
        {
        }
    }
}
