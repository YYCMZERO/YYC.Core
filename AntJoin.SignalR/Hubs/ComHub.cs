using AntJoin.SignalR.Models;
using AntJoin.SignalR.UserManager;

namespace AntJoin.SignalR.Hubs
{
    /// <summary>
    /// 通用Hub
    /// </summary>
    //[Authorize]
    public class ComHub : BaseHub<IHubClient>
    {
        public ComHub(IUserManager assistRedis) : base(assistRedis)
        {
        }
    }

}
