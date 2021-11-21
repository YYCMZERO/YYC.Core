using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Grpc.Core;
using Grpc.Net.Client;

namespace AntJoin.Grpc.Client
{
    public class DefaultGrpcClientFactory : IGrpcClientFactory
    {
        private readonly ConcurrentDictionary<Type, object> _typeIntances;

        public DefaultGrpcClientFactory()
        {
            _typeIntances = new ConcurrentDictionary<Type, object>();
        }
        
        public TGrpcClient Get<TGrpcClient>(string address) where TGrpcClient : class
        {
            var invokeType = typeof(TGrpcClient);
            if (!_typeIntances.TryGetValue(invokeType, out var instance))
            {
                var parameterType = typeof(ChannelBase);
                var channel = GrpcChannel.ForAddress(address);
                var clientType = typeof(TGrpcClient);                                 
                var constructor = clientType
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(c =>
                        c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == parameterType);

                if (constructor != null)
                {
                    instance = constructor.Invoke(new[] {channel});
                    _typeIntances.AddOrUpdate(invokeType, k => instance, (k,v) => instance);
                }
            }

            return (TGrpcClient) instance;
        }
    }
}