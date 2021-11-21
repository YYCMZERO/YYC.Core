namespace AntJoin.SignalR.Models
{
    /// <summary>
    /// 基础内容类
    /// </summary>
    public class BaseHubMessage
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }


        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }


        /// <summary>
        /// 类别
        /// </summary>
        public virtual int Type { get; set; }


        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; }
    }
}
