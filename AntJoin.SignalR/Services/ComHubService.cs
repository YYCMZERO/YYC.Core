using AntJoin.SignalR.Hubs;
using AntJoin.SignalR.UserManager;
using Microsoft.AspNetCore.SignalR;

namespace AntJoin.SignalR.Services
{
    public class ComHubService : BaseHubService<ComHub>, IHubService
    {
        public ComHubService(IHubContext<ComHub> hubContext, IUserManager userManager) : base(hubContext, userManager)
        {
        }
    }
}
