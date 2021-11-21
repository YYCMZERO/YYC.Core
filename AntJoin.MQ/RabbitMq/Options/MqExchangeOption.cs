using System.Collections.Generic;

namespace AntJoin.MQ.Options
{
    /// <summary>
    /// MQ的路由配置参数
    /// </summary>
    public class MqExchangeOption
    {
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 路由类型
        /// 可以通过MqExchangeType提供值   
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// 结构化参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }


        /// <summary>
        /// 初始化
        /// </summary>
        public MqExchangeOption() : this("")
        {

        }


        /// <summary>
        /// 初始化MQ路由机配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public MqExchangeOption(string name, string type = MqExchangeType.Direct)
        {
            Name = name;
            Type = type;
            Durable = true;
            AutoDelete = false;
            Arguments = new Dictionary<string, object>();
        }
    }
}