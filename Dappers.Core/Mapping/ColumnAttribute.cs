using System;
using System.Collections.Generic;
using System.Text;

namespace Dappers.Mapping
{ 
    /// <summary>
    /// 将类与数据库表中的列相关联。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ColumnAttribute : DataAttribute
    {
        /// <summary>
        /// 获取或设置 Dappers.Mapping.AutoSync 枚举。
        /// </summary>
        public AutoSync AutoSync { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示列是否可包含 null 值。
        /// </summary>
        public bool CanBeNull { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal bool CanBeNullSet { get; }

        /// <summary>
        /// 获取或设置数据库列的类型。
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示列是否为数据库中的计算列。
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示列是否包含数据库自动生成的值。
        /// </summary>
        public bool IsDbGenerated { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示列是否包含 LINQ to SQL 继承层次结构的鉴别器值。
        /// </summary>
        public bool IsDiscriminator { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示该类成员是否表示作为表的整个主键或部分主键的列。
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示成员的列类型是否为数据库时间戳或版本号。
        /// </summary>
        public bool IsVersion { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示 LINQ to SQL 如何进行开放式并发冲突的检测。
        /// </summary>
        public UpdateCheck UpdateCheck { get; set; }

        /// <summary>
        /// 初始化 Dappers.Mapping.ColumnAttribute 类的新实例。
        /// </summary>
        public ColumnAttribute()
        {
        }
    }
}
