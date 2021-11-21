using System;

namespace AntJoin.MQ.EventHandlers
{
    /// <summary>
    /// 集成事件，MQ发送事件的基础类
    /// </summary>
    public abstract class IntegratedEvent
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public string EventId { get; set; }
        
        /// <summary>
        /// 事件创建时间
        /// </summary>
        public long CreatedTime { get; set; }


        /// <summary>
        /// 初始化
        /// </summary>
        protected IntegratedEvent()
        {
            EventId = Guid.NewGuid().ToString("N");
            CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}