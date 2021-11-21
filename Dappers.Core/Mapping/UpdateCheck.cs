using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dappers.Mapping
{
    //
    // 摘要:
    //     指定何时测试对象是否有并发冲突。
    public enum UpdateCheck
    {
        /// <summary>
        /// 始终进行检查。此为默认值，除非对某个成员而言 System.Data.Linq.Mapping.ColumnAttribute.IsVersion 为 true
        /// </summary>
        [Description("始终进行检查。此为默认值，除非对某个成员而言 System.Data.Linq.Mapping.ColumnAttribute.IsVersion 为 true")]
        Always = 0,

        /// <summary>
        /// 从不检查
        /// </summary>
        [Description("从不检查")]
        Never = 1,

        /// <summary>
        /// 仅在已更改对象后检查
        /// </summary>
        [Description("仅在已更改对象后检查")]
        WhenChanged = 2
    }
}
