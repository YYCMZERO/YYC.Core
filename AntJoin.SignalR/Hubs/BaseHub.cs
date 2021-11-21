using AntJoin.Core.Extensions;
using AntJoin.SignalR.Models;
using AntJoin.SignalR.UserManager;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AntJoin.SignalR.Hubs
{
    public abstract class BaseHub<T> : Hub<T>, IHubServer where T : class, IHubClient
    {
        private readonly IUserManager _userManager;

        public BaseHub(IUserManager userManager)
        {
            _userManager = userManager;
            _userManager.InitAsync(GetType());
        }

        /// <summary>
        /// 连接的时候
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = GetUserId();
            if (userId > 0)
            {
                await _userManager.LoginAsync(userId, connectionId);
                //进入自身群组
                var groupName = _userManager.GetUserSelfGroupName(userId);
                await JoinGroup(groupName);
                await OnConnectedFuncAsync(userId);
            }
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = GetUserId();
            if (userId > 0)
            {
                //离开自身群组
                var groupName = _userManager.GetUserSelfGroupName(userId);
                await LeavingGroup(groupName);
                await _userManager.LogoutAsync(userId, connectionId);
                await OnDisconnectedFuncAsync(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// 发送消息给单人
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendMessage(UserHubMessage message)
        {
            var groupName = _userManager.GetUserSelfGroupName(message.ToUserId);
            await Clients.Group(groupName).ReceiveMessage(message);
        }


        /// <summary>
        /// 加入群组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task JoinGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, groupName);
        }


        /// <summary>
        /// 离开群组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task LeavingGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.RemoveFromGroupAsync(connectionId, groupName);
        }


        /// <summary>
        /// 发送消息给群组
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendGroupMessage(GroupHubMessage message)
        {
            await Clients.Group(message.ToGroupName).ReceiveGroupMessages(message);
        }


        /// <summary>
        /// 广播信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendAllMessage(BaseHubMessage message)
        {
            await Clients.All.ReceiveAllMessages(message);
        }


        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <returns></returns>
        protected long GetUserId()
        {
            var httpContext = Context.GetHttpContext();
            long userId = httpContext.GetLoginUser().UserId;
            if (userId > 0)
                return userId;
            if (long.TryParse(Context.UserIdentifier, out userId) && userId > 0)
                return userId;
            if (long.TryParse(httpContext.Request.Query["userId"].ToString(), out userId) && userId > 0)
                return userId;
            return 0;
        }


        #region 扩展方法
        /// <summary>
        /// 连接方法
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected virtual async Task OnConnectedFuncAsync(long userId)
        {
            await Task.Run(() => { });
        }


        /// <summary>
        /// 断开连接方法
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected virtual async Task OnDisconnectedFuncAsync(long userId)
        {
            await Task.Run(() => { });
        }
        #endregion
    }
}
