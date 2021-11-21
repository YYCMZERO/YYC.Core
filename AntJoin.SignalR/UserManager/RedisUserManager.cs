using AntJoin.Redis;
using System;
using System.Threading.Tasks;

namespace AntJoin.SignalR.UserManager
{
    public class RedisUserManager : IUserManager
    {
        private readonly IRedisClient _redisClient;

        public RedisUserManager(IRedisClientProvider redisClientProvider)
        {
            _redisClient = redisClientProvider.Get(SignalRConstants.SignalRClientName);
        }


        //2个都存，方便根据ConnectionId查找到对应的UserId
        private string _userKeyPrev;
        private string _userKeyListKey;
        private readonly TimeSpan _expiryTimeSpan = new TimeSpan(24, 0, 0);

        public void InitAsync(Type type)
        {
            var name = type.Name;
            _userKeyPrev = "SignalR:" + name + ":UserId:";
            _userKeyListKey = "SignalR:" + name + ":UserKeyList";
        }

        /// <summary>
        /// 登录，保存用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        public async Task LoginAsync(long userId, string connectionId)
        {
            if (_redisClient != null)
            {
                var key = _userKeyPrev + userId;
                await _redisClient.StringSet(key, connectionId, _expiryTimeSpan);
                await _redisClient.ListLeftPush(_userKeyListKey, key);
            }
        }


        /// <summary>
        /// 登出，清除用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        public async Task LogoutAsync(long userId, string connectionId)
        {
            if (_redisClient != null)
            {
                var key = _userKeyPrev + userId;
                await _redisClient.KeyDelete(key);
                await _redisClient.ListRemove(_userKeyListKey, key);
            }
        }


        /// <summary>
        /// 获取当前用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetConnectionCountAsync() 
        {
            var l = await _redisClient.ListLength(_userKeyListKey);
            var keys = await _redisClient.ListRange<string>(_userKeyListKey, 0, l);
            foreach (var key in keys)
            {
                if (!await _redisClient.KeyExists(key))
                    await _redisClient.ListRemove(_userKeyListKey, key);
            }
            return await _redisClient.ListLength(_userKeyListKey);
        }


        /// <summary>
        /// 判断当前用户是否连接
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsConnectionAsync(long userId) => await _redisClient.KeyExists(_userKeyPrev + userId);


        /// <summary>
        /// 获取用户自身组名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserSelfGroupName(long userId) => SignalRConstants.UserSelfGroupNamePrev + userId;

    }
}
