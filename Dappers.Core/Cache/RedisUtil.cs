using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dappers.Cache
{
    class RedisUtil
    {
        private static Encoding _textEncoding = Encoding.UTF8;
        private static ConnectionMultiplexer _connection = null;
        private static readonly object SyncObject = new object();

        public static ConnectionMultiplexer GetFactionConn()
        {
            if (_connection == null || !_connection.IsConnected)
            {
                lock (SyncObject)
                {
                    var redisConfig = new RedisConfig();
                    var configurationOptions = new ConfigurationOptions()
                    {
                        Password = redisConfig.Password,
                        EndPoints = { { redisConfig.Host, Convert.ToInt32(redisConfig.Port) } },
                        AbortOnConnectFail = false,
                        KeepAlive = 200,
                        ConnectRetry = 5,
                        ConnectTimeout = 5000,
                        SyncTimeout = 3000
                    };
                    _connection = ConnectionMultiplexer.Connect(configurationOptions);

                }
            }
            return _connection;
        }        

        /// <summary>
        /// 关闭连接
        /// </summary>
        public static void DisableConnect()
        {
            if (_connection == null || !_connection.IsConnected)
            {
                RedisUtil.GetFactionConn().Dispose();
            }
        }
        /// <summary>
        /// 获取Redis连接
        /// </summary>
        public static IDatabase SingleClient
        {
            get
            {
                return RedisUtil.GetFactionConn().GetDatabase();
            }
        }
    }
}
