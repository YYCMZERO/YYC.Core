using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AntJoin.Grpc.Server
{
    public interface IGrpcEndpointBuilder
    {
        /// <summary>
        /// 构建GRPC节点
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        void Build<T>(IEndpointRouteBuilder endpoint,Action<GrpcServiceEndpointConventionBuilder> action = null) where T : IGrpcService;
    }
}