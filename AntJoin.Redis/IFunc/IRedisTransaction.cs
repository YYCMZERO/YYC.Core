using StackExchange.Redis;

namespace AntJoin.Redis
{
    public interface IRedisTransaction
    {
        /// <summary>
        /// 启动事务
        /// </summary>
        /// <returns></returns>
        ITransaction BeginTransaction();



        /// <summary>
        /// 获取Redis库
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase();
    }
}
