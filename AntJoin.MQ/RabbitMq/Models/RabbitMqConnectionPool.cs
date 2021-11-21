using System;
using System.Collections.Concurrent;

namespace AntJoin.MQ.Models
{
    internal class RabbitMqConnectionPool : IDisposable
    {
        internal static readonly RabbitMqConnectionPool Instance;
        private readonly ConcurrentDictionary<string, RabbitMqConnection> _connections;

        static RabbitMqConnectionPool()
        {
            Instance = new RabbitMqConnectionPool();
        }

        private RabbitMqConnectionPool()
        {
            _connections = new ConcurrentDictionary<string, RabbitMqConnection>();
        }

        /// <summary>
        /// 获取或者添加
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="connection"></param>
        internal RabbitMqConnection Add(string connectionName, RabbitMqConnection connection)
        {
            if (!_connections.TryGetValue(connectionName, out var mqConnection))
            {
                mqConnection = _connections.AddOrUpdate(connectionName, k => connection, (k, v) => v);
            }

            return mqConnection;
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="connection"></param>
        internal void Update(string connectionName, RabbitMqConnection connection)
        {
            _connections.TryUpdate(connectionName, connection, connection);
        }


        /// <summary>
        /// 尝试获取连接
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal bool TryGet(string connectionName, out RabbitMqConnection connection)
        {
            return _connections.TryGetValue(connectionName, out connection);
        }

        public void Dispose()
        {
            foreach (var (_, value) in _connections)
            {
                value.Connection.Close();
                value.Connection.Dispose();
                value.Connection = null;
            }

            _connections.Clear();
        }
    }
}