using AntJoin.Grpc.Client;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GrpcClientExtensions
    {
        private static readonly Type GrpcClientExtensionType = typeof(GrpcClientServiceExtensions);
        
        /// <summary>
        /// 添加客户端工厂
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClientFactory(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.Add(ServiceDescriptor.Singleton<IGrpcClientFactory, DefaultGrpcClientFactory>());
            return services;
        }

        /// <summary>
        /// 添加grpc客户端
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="configureClient"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, Assembly assembly = null
             , Action<GrpcClientFactoryOptions> configureClient = null)
        {
            return services.AddGrpcClients<IGrpcClient>(assembly, configureClient);
        }

        /// <summary>
        /// 添加Grpc客户端
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="configureClient"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClients<TService>(this IServiceCollection services, Assembly assembly = null
             , Action<GrpcClientFactoryOptions> configureClient = null) where TService : IGrpcClient
        {
            IEnumerable<Type> Scan<T>()
            {
                var findType = typeof(T);
                assembly ??= Assembly.GetEntryAssembly();

                return assembly.GetExportedTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType && t.IsClass &&
                                findType.IsAssignableFrom(t))
                    .ToList();
            }

            var types = Scan<TService>();

            var invokeMethod = GrpcClientExtensionType.GetMethod("AddGrpcClient"
                , new Type[] { typeof(IServiceCollection), typeof(Action<GrpcClientFactoryOptions>) });
            if (invokeMethod != null)
            {
                foreach (var type in types)
                {
                    var method = invokeMethod.MakeGenericMethod(type);
                    var builder = (IHttpClientBuilder)method.Invoke(null, new object[] { services, configureClient });
                    builder?.EnableCallContextPropagation();
                }
            }
            return services;
        }


        /// <summary>
        /// 添加Grpc客户端，可用构造函数注入客户端类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="configureClient"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClientList(this IServiceCollection services, Assembly assembly = null, Action<GrpcClientFactoryOptions> configureClient = null)
        {
            IEnumerable<Type> Scan<T>()
            {
                var findType = typeof(T);
                assembly ??= Assembly.GetEntryAssembly();
                return assembly.GetExportedTypes().Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType && t.IsClass && findType.IsAssignableFrom(t)).ToList();
            }
            var types = Scan<ClientBase>();
            var invokeMethod = GrpcClientExtensionType.GetMethod("AddGrpcClient"
                , new Type[] { typeof(IServiceCollection), typeof(Action<GrpcClientFactoryOptions>) });
            if (invokeMethod != null)
            {
                foreach (var type in types)
                {
                    var method = invokeMethod.MakeGenericMethod(type);
                    method.Invoke(null, new object[] { services, configureClient });
                }
            }
            return services;
        }
    }
}