using System;

namespace AntJoin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableAttribute : Attribute
    {

        /// <summary>
        /// 表格名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否加入缓存读写
        /// </summary>
        public bool IsCacheReadWrite { get; set; } = false;

        /// <summary>
        /// 缓存过期时间，单位秒
        /// </summary>
        public int CacheExpireTime { get; set; }
    }
}
