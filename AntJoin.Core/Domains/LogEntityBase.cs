using System;

namespace AntJoin.Core.Domains
{
    /// <summary>
    /// 日志实体基类
    /// </summary>
    public class LogEntityBase
    {
        /// <summary>
        /// 日志编号
        /// </summary>
        public virtual string ID { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public virtual DateTime? Time { get; set; }
    }
}
