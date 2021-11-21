using System.Collections.Generic;
using System.Threading.Tasks;
using AntJoin.SignalR.Models;

namespace AntJoin.SignalR.Services
{
    public interface IHubService
    {

        /// <summary>
        /// 发送消息给单人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        Task SendMessageAsync(long userId, BaseHubMessage message, string methodName = "ReceiveMessage");


        /// <summary>
        /// 发送消息给多人
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        Task SendMessagesAsync(List<long> userIds, BaseHubMessage message, string methodName = "ReceiveMessages");


        /// <summary>
        /// 发送消息给群组
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        Task SendGroupMessageAsync(string groupName, BaseHubMessage message, string methodName = "ReceiveGroupMessages");


        /// <summary>
        /// 广播信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        Task SendAllMessageAsync(BaseHubMessage message, string methodName = "ReceiveAllMessages");
    }
}
