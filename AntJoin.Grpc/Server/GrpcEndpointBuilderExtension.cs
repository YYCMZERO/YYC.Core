using System;
using AntJoin.Grpc.Server;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class GrpcEndpointBuilderExtension
    {
        /// <summary>
        /// 加载Grpc服务节点，通过这个方法可以自动把所有GRPC服务都加载，
        /// 所有GRPC服务必须实现 <see cref="IGrpcService"/> 该接口
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        public static void MapGrpcEndPoint(this IEndpointRouteBuilder builder,
            Action<GrpcServiceEndpointConventionBuilder> action = null)
        {
            var grpcBuilder = builder.ServiceProvider.GetRequiredService<IGrpcEndpointBuilder>();
            grpcBuilder.Build<IGrpcService>(builder, action);
        }
    }
}