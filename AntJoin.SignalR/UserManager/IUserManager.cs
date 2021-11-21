using System;
using System.Threading.Tasks;

namespace AntJoin.SignalR.UserManager
{
    public interface IUserManager
    {
        void InitAsync(Type type);

        /// <summary>
        /// 登录，保存用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        Task LoginAsync(long userId, string connectionId);


        /// <summary>
        /// 登出，清除用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        Task LogoutAsync(long userId, string connectionId);


        /// <summary>
        /// 获取当前用户数量
        /// </summary>
        /// <returns></returns>
        Task<long> GetConnectionCountAsync();


        /// <summary>
        /// 判断当前用户是否连接
        /// </summary>
        /// <returns></returns>
        Task<bool> IsConnectionAsync(long userId);


        /// <summary>
        /// 获取用户自身组名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserSelfGroupName(long userId);
    }
}
