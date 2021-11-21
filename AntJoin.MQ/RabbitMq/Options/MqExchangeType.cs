namespace AntJoin.MQ.Options
{
    /// <summary>
    /// 路由类型
    /// </summary>
    public class MqExchangeType
    {
        /// <summary>
        /// 类似Direct，区别在route-key可以使用规则去匹配
        /// </summary>
        public const string Topic = "topic";
        
        /// <summary>
        /// 一对一类型，需要指定 route-key
        /// </summary>
        public const string Direct = "direct";
        
        /// <summary>
        /// 广播方式的订阅，不需要传递RouteKey
        /// </summary>
        public const string Fanout = "fanout";
    }
}