using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntJoin.SignalR.Models;
using AntJoin.SignalR.UserManager;
using Microsoft.AspNetCore.SignalR;

namespace AntJoin.SignalR.Services
{
    public abstract class BaseHubService<T> where T : Hub
    {
        private readonly IHubContext<T> _hubContext;
        private readonly IUserManager _userManager;

        protected BaseHubService(IHubContext<T> hubContext, IUserManager userManager)
        {
            _hubContext = hubContext;
            _userManager = userManager;
            _userManager.InitAsync(typeof(T));
        }


        /// <summary>
        /// 发送消息给单人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(long userId, BaseHubMessage message, string methodName = "ReceiveMessage")
        {
            var groupName = _userManager.GetUserSelfGroupName(userId);
            await _hubContext.Clients.Group(groupName).SendAsync(methodName, message);
        }



        /// <summary>
        /// 发送消息给多人
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task SendMessagesAsync(List<long> userIds, BaseHubMessage message, string methodName = "ReceiveMessages")
        {
            List<string> connectionIdList = new List<string>();
            var groupList = userIds.Select(userId => _userManager.GetUserSelfGroupName(userId)).ToList();
            await _hubContext.Clients.Groups(groupList).SendAsync(methodName, message);
        }

        ///// <summary>
        ///// 加入群组
        ///// </summary>
        ///// <param name="groupName"></param>
        ///// <returns></returns>
        //public virtual async Task JoinGroup(string groupName)
        //{
        //    await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        //}


        ///// <summary>
        ///// 离开群组
        ///// </summary>
        ///// <param name="groupName"></param>
        ///// <returns></returns>
        //public virtual async Task LeavingGroup(string groupName)
        //{
        //    var connectionId = Context.ConnectionId;
        //    await _hubContext.Clients.RemoveFromGroupAsync(connectionId, groupName);
        //}


        /// <summary>
        /// 发送消息给群组
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task SendGroupMessageAsync(string groupName, BaseHubMessage message, string methodName = "ReceiveGroupMessages")
        {
            await _hubContext.Clients.Group(groupName).SendAsync(methodName, message);
        }


        /// <summary>
        /// 广播信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task SendAllMessageAsync(BaseHubMessage message, string methodName = "ReceiveAllMessages")
        {
            await _hubContext.Clients.All.SendAsync(methodName, message);
        }
    }
}
