using AntJoin.Redis;
using AntJoin.SignalR.Services;
using AntJoin.SignalR.UserManager;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AntJoin.SignalR.Extensions
{
    public static class SignalRServerExtension
    {
        /// <summary>
        /// 添加SignalR服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisOption">redis配置</param>
        /// <returns></returns>
        public static IServiceCollection AddSignalRService(this IServiceCollection services, ConnectionOption redisOption)
        {
            if (redisOption != null)
            {
                #region SignalR添加redis底板
                redisOption.RedisClientName = SignalRConstants.SignalRClientName;
                var configurationOptions = new ConfigurationOptions
                {
                    AllowAdmin = true,
                    ClientName = SignalRConstants.SignalRClientName,
                    Password = redisOption.Password,
                    AbortOnConnectFail = false,
                    KeepAlive = 200,
                    ConnectTimeout = 5000,
                    SyncTimeout = 10000,
                    DefaultDatabase = redisOption.DefaultDb,
                    SocketManager = new SocketManager(workerCount: redisOption.WorkerCount)
                };
                foreach (var endPoint in redisOption.EndPoints)
                {
                    configurationOptions.EndPoints.Add(endPoint.Host, endPoint.Port);
                }
                services.AddSignalR().AddStackExchangeRedis(options =>
                {
                    options.Configuration = configurationOptions;
                });
                #endregion

                services.AddRedis(redisOption);
            }
            else
                services.AddSignalR();
            services.AddScoped<IUserManager, RedisUserManager>();
            services.AddScoped<IHubService, ComHubService>();
            return services;
        }
    }
}
