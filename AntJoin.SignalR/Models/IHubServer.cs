using System.Threading.Tasks;

namespace AntJoin.SignalR.Models
{
    /// <summary>
    /// 定义 Hub 契约
    /// 定义一个客户端方法的接口以实现强类型的客户端方法调用
    /// </summary>
    public interface IHubServer
    {
        /// <summary>
        /// 发送消息给单人
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessage(UserHubMessage message);


        /// <summary>
        /// 加入群组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task JoinGroup(string groupName);


        /// <summary>
        /// 离开群组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task LeavingGroup(string groupName);


        /// <summary>
        /// 发送消息给群组
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendGroupMessage(GroupHubMessage message);


        /// <summary>
        /// 广播信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendAllMessage(BaseHubMessage message);
    }
}
