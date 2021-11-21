using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dappers.Cache
{
    class StackExchangeHelper
    {
        private static IDatabase _cacheDb = RedisUtil.GetFactionConn().GetDatabase();
        private static readonly Encoding TextEncoding = Encoding.UTF8;

        private static StackExchangeHelper _instance = null;
        private static readonly object Padlock = new object();

        StackExchangeHelper()
        {
        }

        public static StackExchangeHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new StackExchangeHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #region key

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public static bool KeyDelete(string key)
        {
            return _cacheDb.KeyDelete(key);
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public static bool KeyExists(string key)
        {
            return _cacheDb.KeyExists(key);
        }
        
        #endregion 
    }
}

