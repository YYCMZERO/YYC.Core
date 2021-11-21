using System.Collections.Generic;

namespace AntJoin.MQ.Options
{
    /// <summary>
    /// MQ队列配置
    /// </summary>
    public class MqQueueOption
    {
        /// <summary>
        /// 队列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// 是否独享
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// 结构化参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// 预处理数量，可以提升QOS
        /// </summary>
        public ushort PrefetchCount { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public MqQueueOption() : this("")
        {

        }

        /// <summary>
        ///  初始化队列配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefetchCount"></param>
        public MqQueueOption(string name, ushort prefetchCount = 0)
        {
            Name = name;
            PrefetchCount = prefetchCount;
            Durable = true;
            AutoDelete = false;
            Exclusive = false;
            Arguments = new Dictionary<string, object>();
        }
    }
}