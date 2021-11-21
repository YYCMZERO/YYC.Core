namespace AntJoin.SignalR.Models
{
    /// <summary>
    /// 发送给群组
    /// </summary>
    public sealed class GroupHubMessage : BaseHubMessage
    {
        /// <summary>
        /// 群组名
        /// </summary>
        public string ToGroupName { get; set; }
    }
}
