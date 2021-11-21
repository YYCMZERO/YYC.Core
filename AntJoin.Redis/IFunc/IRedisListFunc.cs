using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    /// <summary>
    /// 列表(List)相关操作
    /// </summary>
    public interface IRedisListFunc
    {
        /// <summary>
        /// 获取列表中某个位置的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="index">位置</param>
        /// <returns></returns>
        Task<T> ListGetByIndex<T>(string key, long index);

        /// <summary>
        /// 在 指定元素 后面 插入一个 元素 插入成功返回的列表总长度，插入失败返回-1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pivot">指定元素</param>
        /// <param name="value">要插入的元素</param>
        /// <returns></returns>
        Task<long> ListInsertAfter<T>(string key, T pivot, T value);

        /// <summary>
        /// 在 指定元素 前面 插入一个 元素 插入成功返回列表总长度，插入失败返回-1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pivot">指定元素</param>
        /// <param name="value">要插入的元素</param>
        /// <returns></returns>
        Task<long> ListInsertBefore<T>(string key, T pivot, T value);

        /// <summary>
        /// 从列表的左侧取出一个值 
        /// [ ←| ]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ListLeftPop<T>(string key);

        /// <summary>
        /// 从列表的左侧插入值 
        /// [ →| ]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<long> ListLeftPush<T>(string key, T value);

        /// <summary>
        /// 从列表的左侧插入多个值 
        /// [ →| ]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Task<long> ListLeftPush<T>(string key, List<T> values);

        /// <summary>
        /// 获取列表中的元素个数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> ListLength(string key);

        /// <summary>
        /// 取出列表中的值从 start 到 end 的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end">-1表示全部取出</param>
        /// <returns></returns>
        Task<List<T>> ListRange<T>(string key, long start = 0L, long end = -1L);

        /// <summary>
        /// 取出列表中的值从 start 到 end 的数据,并转换成字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<List<string>> ListStringRange(string key, long start = 0, long end = -1);

        /// <summary>
        /// 删除列表中的一个元素，可设置要删除的数量 返回删除的数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<long> ListRemove<T>(string key, T value, long count = 0L);

        /// <summary>
        /// 从列表的右侧取出一个值 
        /// [ |→ ]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ListRightPop<T>(string key);

        /// <summary>
        /// source列表的右侧取出值并把值插入到destination的左侧,该操作为原子操作 
        /// [ | → | ]
        /// </summary>
        /// <param name="source">要获取数据的key对应的列表</param>
        /// <param name="destination">要把数据插入的对应目标key的列表</param>
        /// <returns></returns>
        Task<T> ListRightPopLeftPush<T>(string source, string destination);

        /// <summary>
        /// 从列表的右侧插入值  
        /// [ |← ]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<long> ListRightPush<T>(string key, T value);

        /// <summary>
        /// 从列表的右侧插入多个值
        ///  [ |← ]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        Task<long> ListRightPush<T>(string key, List<T> values);

        /// <summary>
        /// 设置列表中某个位置的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="index">列表位置</param>
        /// <param name="value">要设置进来的数据</param>
        /// <returns></returns>
        Task ListSetByIndex<T>(string key, long index, T value);

        /// <summary>
        /// 按指定范围裁剪列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task ListTrim(string key, long start, long end);

        /// <summary>
        /// 把key 从此库移动到另一个库
        /// </summary>
        /// <param name="key"></param>
        /// <param name="database">移动目标数据库</param>
        /// <returns></returns>
        Task<bool> KeyMove(string key, int database);

        /// <summary>
        /// 返回Key的剩余过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TimeSpan?> KeyTimeToLive(string key);

        /// <summary>
        /// 随机取一个key
        /// </summary>
        /// <returns></returns>
        Task<string> KeyRandomAsync();

        /// <summary>
        /// 把某个 key 中的值序列化成 byte[]数组，并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<byte[]> KeyDump(string key);

        /// <summary>
        /// 把 keyDump 命令中序列化的值 反序列化保存在key中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">二进制数据</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        Task KeyRestore(string key, byte[] value, TimeSpan? expiry = null);
    }
}
