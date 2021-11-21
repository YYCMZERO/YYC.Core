using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    /// <summary>
    /// 字符串(string)相关查询
    /// </summary>
    public interface IRedisStringFunc
    {
        /// <summary>
        /// 字符串追加，返回追加后的字符串长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">追加内容</param>
        /// <returns></returns>
        Task<long> StringAppend(string key, string value);

        /// <summary>
        /// 为数字累减，值必须是数字类型
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">累减值</param>
        /// <returns></returns>
        Task<long> StringDecrement(string key, long value = 1L);

        /// <summary>
        /// 为数字累减，值必须是数字类型
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">累减值</param>
        /// <returns></returns>
        Task<double> StringDecrement(string key, double value);

        /// <summary>
        /// 获取一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> StringGet<T>(string key);

        /// <summary>
        /// 批量获取记录
        /// </summary>
        /// <param name="keys">键列表</param>
        /// <returns></returns>
        Task<List<T>> StringGet<T>(List<string> keys);

        /// <summary>
        /// 获取原来的String值 并用新值替换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">新值</param>
        /// <returns></returns>
        Task<T> StringGetSet<T>(string key, T value);

        /// <summary>
        /// 获取字符串的值以及key的剩余的过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<(TimeSpan? Expired, T Value)> StringGetWithExpiry<T>(string key);

        /// <summary>
        /// 为数字累加，值必须是数字类型
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">累加值</param>
        /// <returns></returns>
        Task<long> StringIncrement(string key, long value = 1L);

        /// <summary>
        /// 为数字累加，值必须是数字类型
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">累加值</param>
        /// <returns></returns>
        Task<double> StringIncrement(string key, double value);

        /// <summary>
        /// 获取指定key中字符串长度
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<long> StringLength(string key);

        /// <summary>
        /// 新增一条记录，并设置过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        Task<bool> StringSet<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// 批量新增记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">键值对</param>
        /// <returns></returns>
        Task<bool> StringSet<T>(List<KeyValuePair<string, T>> values);
    }
}
