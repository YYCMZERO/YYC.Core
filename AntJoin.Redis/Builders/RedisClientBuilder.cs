using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AntJoin.Redis
{
    internal class RedisClientBuilder : IRedisClientBuilder
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loggerFactory"></param>
        internal RedisClientBuilder(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<RedisClientBuilder>();
        }


        /// <summary>
        /// 构建连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <returns></returns>
        public IRedisClient Build(ConnectionOption option)
        {
            return new RedisClient(option, CreateConnection(option));
        }


        private IConnectionMultiplexer CreateConnection(ConnectionOption option)
        {
            var configurationOptions = new ConfigurationOptions
            {
                AllowAdmin = true,
                ClientName = option.RedisClientName ?? Constants.DefaultRedisClientName,
                Password = option.Password,
                AbortOnConnectFail = false,
                KeepAlive = 200,
                ConnectTimeout = 5000,
                SyncTimeout = 10000,
                DefaultDatabase = option.DefaultDb,
                SocketManager = new SocketManager(workerCount: option.WorkerCount)
            };

            foreach (var endPoint in option.EndPoints)
            {
                configurationOptions.EndPoints.Add(endPoint.Host, endPoint.Port);
            }
            var connection = ConnectionMultiplexer.Connect(configurationOptions.ToString());
            connection.ConnectionFailed += Connection_ConnectionFailed;
            connection.ConnectionRestored += Connection_ConnectionRestored;
            connection.ErrorMessage += Connection_ErrorMessage;
            connection.InternalError += Connection_InternalError;
            return connection;
        }

        private void Connection_InternalError(object sender, InternalErrorEventArgs e)
        {
            _logger?.LogError(e.Exception, $"Connection_InternalError: [{e.EndPoint}] {e.Origin}");
        }

        private void Connection_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _logger?.LogError($"Connection_ErrorMessage: [{e.EndPoint}] ERROR: {e.Message}");
        }

        private void Connection_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger?.LogInformation(e.Exception, $"Connection_ConnectionRestored: [{e.EndPoint}]");
        }

        private void Connection_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger?.LogError(e.Exception, $"Connection_ConnectionFailed: [${e.EndPoint}] {e.FailureType}");
        }
    }
}
