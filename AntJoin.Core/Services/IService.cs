using AntJoin.Core.Domains;
using System.Linq;
using System.Threading.Tasks;
using WL.Think.Dependency;

namespace AntJoin.Core.Services
{
    /// <summary>
    /// 服务类
    /// </summary>
    [IgnoreDependency]
    public interface IService : IScopeDependency
    {
        /// <summary>
        /// 提交保存
        /// </summary>
        /// <returns></returns>
        Task<int> Commit();
    }


    /// <summary>
    /// 服务定义
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [IgnoreDependency]
    public interface IService<TEntity> : IService where TEntity : class, IEntity
    {
        /// <summary>
        /// 查询源
        /// </summary>
        IQueryable<TEntity> Query { get; }
    }



    /// <summary>
    /// 服务定义
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    [IgnoreDependency]
    public interface IService<TEntity, TKey> : IService<TEntity> where TEntity : class, IEntity<TKey>
    {

    }
}
