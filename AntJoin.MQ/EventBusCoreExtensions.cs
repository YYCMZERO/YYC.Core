using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AntJoin.MQ.EventBus;
using AntJoin.MQ.EventHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusCoreExtensions
    {
        /// <summary>
        /// 添加事件总线服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="assemblies">事件定义和事件处理器所在程序集</param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services,IConfiguration configuration,params Assembly[] assemblies)
        {
            services.AddOptions();
            services.AddRabbitMq();
            
            services.Configure<RabbitMqEventBusOption>(configuration.GetSection(EventBusContants.ConfigurationName));
            services.TryAddSingleton<IEventNameProvider, DefaultEventNameProvider>();
            services.TryAddSingleton<IEventSubscriptionsManager,DefaultEventSubscriptionsManager>();
            services.TryAddSingleton<IEventBus,RabbitMqEventBus>();

            assemblies ??= new[]
            {
                Assembly.GetEntryAssembly()
            };
            
            ScanEventHandler(services,assemblies);
            return services;
        }


        /// <summary>
        /// 添加事件总线服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="assembliesPath">事件定义和事件处理器所在程序集的名称</param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration, params string[] assemblyNames)
        {
            var assemblies = assemblyNames.Select(s => Assembly.Load(s));
            return AddEventBus(services, configuration, assemblies.ToArray());
        }



        private static void ScanEventHandler(IServiceCollection services,IEnumerable<Assembly> assemblies)
        {
            var baseType = typeof(IEventHandler);
            var matchType = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType && baseType.IsAssignableFrom(t))
                .ToList();
            
            foreach (var type in matchType)
            {
                services.AddTransient(type);
            }
        }
    }
}