using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AntJoin.Core.Repositories
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> Commit(CancellationToken cancellationToken = default);


        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection Connection();
    }
}
