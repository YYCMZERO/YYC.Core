using System.ComponentModel;

namespace AntJoin.Core.DataAnnotations
{
    //
    // 摘要:
    //     指示运行时如何在执行插入或更新操作后检索值。
    public enum AutoSync
    {
        /// <summary>
        /// 自动选择值
        /// </summary>
        [Description("自动选择值")]
        Default = 0,

        /// <summary>
        /// 始终返回值
        /// </summary>
        [Description("始终返回值")]
        Always = 1,

        /// <summary>
        /// 从不返回值
        /// </summary>
        [Description("从不返回值")]
        Never = 2,

        /// <summary>
        /// 仅在执行插入操作后返回值
        /// </summary>
        [Description("仅在执行插入操作后返回值")]
        OnInsert = 3,

        /// <summary>
        /// 仅在执行插入操作后返回值
        /// </summary>
        [Description("仅在执行更新操作后返回值")]
        OnUpdate = 4
    }
}
