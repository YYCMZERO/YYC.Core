using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    public interface IRedisKeyFunc
    {
        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<bool> KeyDelete(string key);


        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">Redis Key</param>
        /// <param name="flags"></param>
        /// <returns>成功删除的个数</returns>
        public Task<long> KeyDelete(List<string> keys);


        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> KeyExists(string key);


        /// <summary>
        /// 判断多个key是否存在
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<long> KeyExists(List<string> keys);


        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<bool> KeyExpire(string key, TimeSpan? expiry);


        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<bool> KeyExpire(string key, DateTime? expiry);


        /// <summary>
        /// 重命名KEY
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newKey"></param>
        /// <param name="when"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<bool> KeyRenameAsync(string key, string newKey);

        /// <summary>
        /// 移除Key的生存时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> KeyPersist(string key);
    }
}
