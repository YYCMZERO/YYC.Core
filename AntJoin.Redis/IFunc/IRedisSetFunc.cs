using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    public interface IRedisSetFunc
    {
        /// <summary>
        /// 向集合中添加一个元素。返回是否添加成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> SetAdd<T>(string key, T value);

        /// <summary>
        /// 向集合中添加一堆元素。 返回添加成功的个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        Task<long> SetAdd<T>(string key, List<T> values);

        /// <summary>
        /// 两集合 交集/并集/差集
        /// </summary>
        /// <param name="operation">集合操作方式</param>
        /// <param name="first">集合1的键</param>
        /// <param name="second">集合2的键</param>
        /// <returns></returns>
        Task<List<T>> SetCombine<T>(SetOperation operation, string first, string second);

        /// <summary>
        /// 多集合 交集/并集/差集
        /// </summary>
        /// <param name="operation">集合操作方式</param>
        /// <param name="keys">多个集合的键</param>
        /// <returns></returns>
        Task<List<T>> SetCombine<T>(SetOperation operation, List<string> keys);

        /// <summary>
        /// 两集合 交集/并集/差集。 操作结果保存在 destination 返回受影响的个数
        /// </summary>
        /// <param name="operation">集合操作方式</param>
        /// <param name="destination">存放结果的目标集合的键</param>
        /// <param name="first">集合1的键</param>
        /// <param name="second">集合2的键</param>
        /// <returns></returns>
        Task<long> SetCombineAndStore(SetOperation operation, string destination, string first, string second);

        /// <summary>
        /// 多集合 交集/并集/差集。操作结果保存在 destination 返回受影响的个数
        /// </summary>
        /// <param name="operation">集合操作方式</param>
        /// <param name="destination">存放结果的目标集合的键</param>
        /// <param name="keys">多个集合的键</param>
        /// <returns></returns>
        Task<long> SetCombineAndStore(SetOperation operation, string destination, List<string> keys);

        /// <summary>
        /// 判断集合中是否存在元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> SetContains<T>(string key, T value);

        /// <summary>
        /// 返回集合中元素的个数
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<long> SetLength(string key);

        /// <summary>
        /// 返回集合中的所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<List<T>> SetMembers<T>(string key);

        /// <summary>
        /// 把一个元素从source集合中移动到 destination集合中 返回是否移动成功
        /// 当destination 中已存在元素，仅从 source中删除元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">要移动元素的集合的键，移动后，元素会从这里删除</param>
        /// <param name="destination">要存放元素的集合的键</param>
        /// <param name="value">移动的元素值</param>
        /// <returns></returns>
        Task<bool> SetMove<T>(string source, string destination, T value);

        /// <summary>
        /// 从集合中随机取出一个元素, 会删除集合中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<T> SetPop<T>(string key);

        /// <summary>
        /// 从集合中取出 count 个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="count">要取出元素的个数</param>
        /// <returns></returns>
        Task<List<T>> SetPop<T>(string key, long count);

        /// <summary>
        /// 从集合中随机返回一个元素, 不删除集合中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<T> SetRandomMember<T>(string key);

        /// <summary>
        /// 从集合中随机返回count的元素, 不删除集合中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="count">要获取的数量</param>
        /// <returns></returns>
        Task<List<T>> SetRandomMembers<T>(string key, long count);

        /// <summary>
        /// 从集合中移除指定的元素 返回是否移除成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">要移除的值</param>
        /// <returns></returns>
        Task<bool> SetRemove<T>(string key, T value);

        /// <summary>
        /// 从集合中移除一堆元素 返回移除元素的个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="values">要移除的值的集合</param>
        /// <returns></returns>
        Task<long> SetRemove<T>(string key, List<T> values);

        /// <summary>
        /// 模糊查找
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        Task<List<T>> SetScan<T>(string key, string pattern = default, int pageSize = 250, long cursor = 0L, int pageOffset = 0);
    }
}
