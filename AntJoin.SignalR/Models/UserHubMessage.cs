namespace AntJoin.SignalR.Models
{
    /// <summary>
    /// 发送给单独用户
    /// </summary>
    public sealed class UserHubMessage : BaseHubMessage
    {
        /// <summary>
        /// 接受人
        /// </summary>
        public long ToUserId { get; set; }
    }
}
