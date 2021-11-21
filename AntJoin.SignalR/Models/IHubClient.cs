using System.Threading.Tasks;

namespace AntJoin.SignalR.Models
{
    /// <summary>
    /// 定义 Hub 契约
    /// 定义一个客户端方法的接口以实现强类型的客户端方法调用
    /// </summary>
    public interface IHubClient
    {
        /// <summary>
        /// 接收私信
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveMessage(BaseHubMessage message);


        /// <summary>
        /// 接收群消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveGroupMessages(BaseHubMessage message);


        /// <summary>
        /// 接收广播
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveAllMessages(BaseHubMessage message);
    }
}
