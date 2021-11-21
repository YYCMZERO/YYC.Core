using System;
using System.Reflection;
using AntJoin.Grpc.Server;
using Grpc.AspNetCore.Server;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GrpcServerExtensions
    {
        /// <summary>
        /// 添加GRPC服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">GRPC服务所在的程序集，没有配置直接去当前程序集</param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcService(this IServiceCollection services,
            Assembly assembly = null,
            Action<GrpcServiceOptions> configureOptions = null)
        {
            if (configureOptions == null)
            {
                services.AddGrpc();
            }
            else
            {
                services.AddGrpc(configureOptions);
            }

            assembly ??= Assembly.GetEntryAssembly();
            services.Add(
                ServiceDescriptor
                    .Singleton<IGrpcEndpointBuilder>(sp => new GrpcEndpointAutomaticBuilder(assembly))
            );

            return services;
        }
    }
}