using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    /// <summary>
    /// Hash的相关操作
    /// </summary>
    public interface IRedisHashFunc
    {
        /// <summary>
        /// 为键中指定字段累减
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="hashField">字段</param>
        /// <param name="value">累减值</param>
        /// <returns></returns>
        Task<long> HashDecrement(string key, string hashField, long value = 1L);

        /// <summary>
        /// 为键中指定字段累减
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">字段</param>
        /// <param name="value">累减值</param>
        /// <returns></returns>
        Task<double> HashDecrement(string key, string hashField, double value);

        /// <summary>
        /// 删除某个键
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">字段，需要删除的字段</param>
        /// <returns></returns>
        Task<bool> HashDelete(string key, string hashField);

        /// <summary>
        /// 批量删除字段
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashFields">字段集合，需要删除的字段集合</param>
        /// <returns></returns>
        Task<long> HashDelete(string key, List<string> hashFields);

        /// <summary>
        /// 判断指定键是否存在此字段
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">字段</param>
        /// <returns></returns>
        Task<bool> HashExists(string key, string hashField);

        /// <summary>
        /// 获取指定键的某个字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="hashField">字段</param>
        /// <returns></returns>
        Task<T> HashGet<T>(string key, string hashField);

        /// <summary>
        /// 获取指定键的多个字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="hashFields">字段集合</param>
        /// <returns></returns>
        Task<List<T>> HashGet<T>(string key, List<string> hashFields);

        /// <summary>
        /// 获取指定键所有字段的所有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<List<(string Name,T Value)>> HashGetAll<T>(string key);

        /// <summary>
        /// 为键中指定字段累减
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">字段</param>
        /// <param name="value">累加值</param>
        /// <returns></returns>
        Task<long> HashIncrement(string key, string hashField, long value = 1L);

        /// <summary>
        /// 为键中指定字段累减
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">字段</param>
        /// <param name="value">累加值</param>
        /// <returns></returns>
        Task<double> HashIncrementAsync(string key, string hashField, double value);

        /// <summary>
        /// 获取指定键的所有字段
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<List<string>> HashKeys(string key);

        /// <summary>
        /// 获取指定键中字段数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<long> HashLength(string key);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="T">键</typeparam>
        /// <param name="key"></param>
        /// <param name="values">新增数据，key是字段，value是数据</param>
        /// <returns></returns>
        Task HashSet<T>(string key, List<KeyValuePair<string,T>> values);

        /// <summary>
        /// 新增一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="hashField">字段</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> HashSet<T>(string key, string hashField, T value);

        /// <summary>
        /// 获取键中所有字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<List<T>> HashValues<T>(string key);
    }
}
