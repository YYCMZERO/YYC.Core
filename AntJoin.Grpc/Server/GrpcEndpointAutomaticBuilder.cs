using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AntJoin.Grpc.Server
{
    public class GrpcEndpointAutomaticBuilder : IGrpcEndpointBuilder
    {
        private readonly Type _invokeType = typeof(GrpcEndpointRouteBuilderExtensions);
        private readonly Assembly _assembly;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assembly">grpc服务定义所在程序集</param>
        public GrpcEndpointAutomaticBuilder(Assembly assembly)
        {
            _assembly = assembly;
        }


        public void Build<T>(IEndpointRouteBuilder endpoint,Action<GrpcServiceEndpointConventionBuilder> action = null) where T : IGrpcService
        {
            var matchTypes = Scan<T>();
            var invokeMethodType = _invokeType.GetMethod("MapGrpcService");
            if (invokeMethodType == null)
            {
                throw new MissingMethodException("can not found MapGrpcService function");
            }

            foreach (var matchType in matchTypes)
            {
                var methodType = invokeMethodType.MakeGenericMethod(matchType);
                var builder = (GrpcServiceEndpointConventionBuilder) methodType.Invoke(null, new[] {endpoint});
                action?.Invoke(builder);
            }
        }

        /// <summary>
        /// 扫描类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerable<Type> Scan<T>() where T : IGrpcService
        {
            var findType = typeof(T);
            return _assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericType && t.IsClass &&
                            findType.IsAssignableFrom(t))
                .ToList();
        }
    }
}