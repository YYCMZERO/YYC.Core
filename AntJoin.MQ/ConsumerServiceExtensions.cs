using AntJoin.MQ.EventBus;
using AntJoin.MQ.EventHandlers;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsumerServiceExtensions
    {
        /// <summary>
        /// 添加订阅服务
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        public static IApplicationBuilder AddSubscibuteService<TEvent, THandler>(this IApplicationBuilder app)
            where TEvent : IntegratedEvent where THandler : IIntegrateEventHandler<TEvent>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var eventBus = scope.ServiceProvider.GetService<IEventBus>();
            eventBus.Subscribe<TEvent, THandler>();
            return app;
        }
    }
}