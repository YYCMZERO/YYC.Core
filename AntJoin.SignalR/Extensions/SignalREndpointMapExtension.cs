using AntJoin.SignalR.Hubs;
using AntJoin.SignalR.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Reflection;

namespace AntJoin.SignalR.Extensions
{
    public static class SignalREndpointMapExtension
    {
        private readonly static Type _invokeType = typeof(HubEndpointRouteBuilderExtensions);

        /// <summary>
        /// 批量注入
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        public static void MapHubcEndPoint(this IEndpointRouteBuilder builder, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var list = types.Where(a => a.IsClass && !a.IsAbstract && !a.IsGenericType && (typeof(BaseHub<IHubClient>).IsAssignableFrom(a))).ToList();
            var invokeMethodType = _invokeType.GetMethod("MapHub", new[] { typeof(IEndpointRouteBuilder), typeof(string) });
            foreach (var item in list)
            {
                var methodType = invokeMethodType.MakeGenericMethod(item);
                var url = SignalRConstants.HubsUrlPrev + item.Name.ToLower().Replace("hub", "");
                methodType.Invoke(null, new object[] { builder, url });
            }
        }
    }
}
