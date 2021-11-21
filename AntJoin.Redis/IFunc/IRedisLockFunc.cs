using System;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    /// <summary>
    /// Redis分布式锁相关操作
    /// </summary>
    public interface IRedisLockFunc
    {
        /// <summary>
        /// 获取一把分布式锁, 获取到返回true 没获取到返回false
        /// 分布式锁循环调用 LockTake 直到返回true 设置一个过期时间,是为了宕机保护,
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<bool> LockTake<T>(string key, T value, TimeSpan expiry);

        /// <summary>
        /// 开启子线程去调用LockExtend 为分布式续航，true 就继续续航 false 不再执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<bool> LockExtend<T>(string key, T value, TimeSpan expiry);

        /// <summary>
        /// 获取分布式锁中的value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> LockQuery<T>(string key);

        /// <summary>
        /// 释放分布式锁
        /// 执行完代码后 使用 LockRelease 释放掉分布式锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> LockRelease<T>(string key, T value);
    }
}
