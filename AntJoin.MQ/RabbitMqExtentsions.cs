using System;
using AntJoin.MQ.Contracts;
using AntJoin.MQ.Implements;
using AntJoin.MQ.Models;
using AntJoin.MQ.Options;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitMqExtentsions
    {
        /// <summary>
        /// 添加RabbitMQ基础服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Transient<IConnectionStrategy, DefaultConnectionStrategy>());
            services.Add(ServiceDescriptor.Transient<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>());
            services.Add(ServiceDescriptor.Transient<IRabbitMqChannelFactory, RabbitMqChannelFactory>());
            return services;
        }


        /// <summary>
        /// 添加RabbitMQ客户端
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqClient(this IServiceCollection services,
            Action<MqConnectionOption> action)
        {
            services.AddRabbitMq();
            var option = new MqConnectionOption("guest", "guest");
            action(option);
            services.AddRabbitMqClient(option);
            return services;
        }


        /// <summary>
        /// 添加RabbitMQ客户端
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqClient(this IServiceCollection services, MqConnectionOption option)
        {
            services.AddRabbitMq();
            RabbitMqConnectionPool.Instance.Add(option.ConnectionName, new RabbitMqConnection(option));
            return services;
        }
    }
}