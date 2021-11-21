using System;

namespace AntJoin.MQ.EventHandlers
{
    /// <summary>
    /// 订阅
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// 事件
        /// </summary>
        public Type Event { get; set; }
        
        /// <summary>
        /// 处理器
        /// </summary>
        public Type EventHandler { get; set; }
    }
}