using System;
using System.Collections.Generic;

namespace AntJoin.MQ.Options
{
    /// <summary>
    /// MQ连接参数
    /// </summary>
    public class MqConnectionOption
    {
        /// <summary>
        /// 连接用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 连接密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 连接账号
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Automatic connection recovery option.
        /// </summary>
        public bool AutomaticRecoveryEnabled { get; set; }

        /// <summary>
        /// Topology recovery option.
        /// </summary>
        public bool TopologyRecoveryEnabled { get; set; }

        /// <summary>
        /// Timeout for connection attempts.
        /// </summary>
        public TimeSpan RequestedConnectionTimeout { get; set; }

        /// <summary>
        /// Heartbeat timeout.
        /// </summary>
        public TimeSpan RequestedHeartbeat { get; set; }

        /// <summary>
        /// 连接节点，对于集群需要输入多个连接节点
        /// </summary>
        public List<MqEndPoint> EndPoints { get; set; }


        /// <summary>
        /// 创建连接
        /// </summary>
        public MqConnectionOption() : this(MqConstacts.DefaultConnectionName)
        {

        }


        /// <summary>
        /// 创建连接
        /// </summary>
        public MqConnectionOption(string connectionName, string username = "guest", string password = "guest", string virtualHost = "antjoin")
        {
            ConnectionName ??= MqConstacts.DefaultConnectionName;
            UserName = username;
            Password = password;
            VirtualHost = virtualHost;
            EndPoints = new List<MqEndPoint>();
            TopologyRecoveryEnabled = true;
            AutomaticRecoveryEnabled = true;
            RequestedConnectionTimeout = TimeSpan.FromMilliseconds(60000);
            RequestedHeartbeat = TimeSpan.FromSeconds(60);
        }
    }
}