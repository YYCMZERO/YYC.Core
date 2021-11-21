using AntJoin.Core.Repositories;
using AntJoin.Repository;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 仓储服务扩展
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// 添加工作单元服务
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="actionBuilder">数据库上下文配置</param>
        /// <param name="maxPoolSize">最大连接池</param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWork<TService, TImplementation>(this IServiceCollection services, Action<DbContextOptionsBuilder> actionBuilder, int maxPoolSize = 128)
            where TService : class, IUnitOfWork
            where TImplementation : UnitOfWork, TService
        {
            services.AddDbContextPool<TService, TImplementation>(actionBuilder, maxPoolSize);
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(ReadOnlyRepository<,>));
            services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));
            services.AddDependency();
            return services;
        }
    }
}
