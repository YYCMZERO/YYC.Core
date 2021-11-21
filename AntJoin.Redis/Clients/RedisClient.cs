using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    /// <summary>
    /// Redis缓存客户端
    /// </summary>
    public class RedisClient : IRedisClient
    {
        #region 字段和构造
        /// <summary>
        /// Redis多连接复用器
        /// </summary>
        private readonly Lazy<IConnectionMultiplexer> _connection;

        /// <summary>
        /// 连接参数
        /// </summary>
        private readonly ConnectionOption _option;


        /// <summary>
        /// 缓存数据库
        /// </summary>
        private IDatabase _db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="option"></param>
        /// <param name="connection"></param>
        internal RedisClient(ConnectionOption option, IConnectionMultiplexer connection)
        {
            _option = option;
            _connection = new Lazy<IConnectionMultiplexer>(connection);
            _db = _connection.Value.GetDatabase();
        } 
        #endregion

        #region Db
        public IRedisClient SelectDb(int dbName = -1)
        {
            _db = _connection.Value.GetDatabase(dbName);
            return this;
        }



        public IRedisClient ResetDb()
        {
            _db = _connection.Value.GetDatabase(_option.DefaultDb);
            return this;
        } 
        #endregion

        #region 缓存KEY
        public async Task<bool> KeyDelete(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyDeleteAsync(key);
        }


        public async Task<long> KeyDelete(List<string> keys)
        {
            Check.NotListNull(keys, nameof(keys));
            var rkeys = ToRedisKeys(keys);
            return await _db.KeyDeleteAsync(rkeys);
        }


        public async Task<bool> KeyExists(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyExistsAsync(key);
        }


        public async Task<long> KeyExists(List<string> keys)
        {
            Check.NotListNull(keys, nameof(keys));
            var rkeys = ToRedisKeys(keys);
            return await _db.KeyExistsAsync(rkeys);
        }


        public async Task<bool> KeyExpire(string key, TimeSpan? expiry)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyExpireAsync(key, expiry);
        }


        public async Task<bool> KeyExpire(string key, DateTime? expiry)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyExpireAsync(key, expiry);
        }


        public async Task<bool> KeyRenameAsync(string key, string newKey)
        {
            Check.NotNull(key, nameof(key));
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            newKey = AddKeyPrefix(newKey);
            return await _db.KeyRenameAsync(key, newKey);
        }


        public async Task<bool> KeyPersist(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyPersistAsync(key);
        }

        public async Task<bool> KeyMove(string key, int database)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyMoveAsync(key, database);
        }

        public async Task<TimeSpan?> KeyTimeToLive(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyTimeToLiveAsync(key);
        }

        public async Task<string> KeyRandomAsync()
        {
            return await _db.KeyRandomAsync();
        }

        public async Task<byte[]> KeyDump(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.KeyDumpAsync(key);
        }

        public async Task KeyRestore(string key, byte[] value, TimeSpan? expiry = null)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            await _db.KeyRestoreAsync(key, value);
        }
        #endregion

        #region String 字符串
        public async Task<long> StringAppend(string key, string value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.StringAppendAsync(key, value);
        }


        public async Task<long> StringDecrement(string key, long value = 1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.StringDecrementAsync(key, value);
        }


        public async Task<double> StringDecrement(string key, double value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.StringIncrementAsync(key, value);
        }

        public async Task<T> StringGet<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.StringGetAsync(key);
            return ToObject<T>(ret);
        }

        public async Task<List<T>> StringGet<T>(List<string> keys)
        {
            Check.NotListNull(keys, nameof(keys));
            var rkeys = ToRedisKeys(keys);
            var ret = await _db.StringGetAsync(rkeys);
            return ret.Select(s => ToObject<T>(s)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<T> StringGetSet<T>(string key, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.StringGetSetAsync(key, ToRedisValue(value));
            return ToObject<T>(ret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<(TimeSpan? Expired, T Value)> StringGetWithExpiry<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.StringGetWithExpiryAsync(key);
            return (ret.Expiry, ToObject<T>(ret.Value));
        }


        public async Task<long> StringIncrement(string key, long value = 1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.StringIncrementAsync(key, value);
        }


        public async Task<double> StringIncrement(string key, double value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.StringIncrementAsync(key, value);
        }


        public async Task<long> StringLength(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.StringLengthAsync(key);
        }


        public async Task<bool> StringSet<T>(string key, T value, TimeSpan? expiry = null)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.StringSetAsync(key, ToRedisValue(value), expiry);
        }


        public async Task<bool> StringSet<T>(List<KeyValuePair<string, T>> values)
        {
            Check.NotListNull(values, nameof(values));
            var items = ToRedisKeyAndRedisValue(values);
            return await _db.StringSetAsync(items);
        }
        #endregion

        #region List 列表
        public async Task<T> ListGetByIndex<T>(string key, long index)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.ListGetByIndexAsync(key, index);
            return ToObject<T>(ret);
        }


        public async Task<long> ListInsertAfter<T>(string key, T pivot, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.ListInsertAfterAsync(key, ToRedisValue(pivot), ToRedisValue(value));
        }


        public async Task<long> ListInsertBefore<T>(string key, T pivot, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.ListInsertBeforeAsync(key, ToRedisValue(pivot), ToRedisValue(value));
        }


        public async Task<T> ListLeftPop<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.ListLeftPopAsync(key);
            return ToObject<T>(ret);
        }


        public async Task<long> ListLeftPush<T>(string key, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.ListLeftPushAsync(key, ToRedisValue(value));
        }


        public async Task<long> ListLeftPush<T>(string key, List<T> values)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var rets = ToRedisValues(values);
            return await _db.ListLeftPushAsync(key, rets);
        }


        public async Task<long> ListLength(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.ListLengthAsync(key);
        }

        public async Task<List<T>> ListRange<T>(string key, long start = 0, long end = -1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.ListRangeAsync(key, start, end);
            return ret.Select(s => ToObject<T>(s)).ToList();
        }

        public async Task<List<string>> ListStringRange(string key, long start = 0, long end = -1)
        {
            RedisValue[] result =await _db.ListRangeAsync(key, start, end);
            string[] array = new string[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                array[i] = Encoding.UTF8.GetString(result[i]);
            }

            return array.ToList();
        }

        public async Task<long> ListRemove<T>(string key, T value, long count = 0)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.ListRemoveAsync(key, ToRedisValue(value), count);
        }

        public async Task<T> ListRightPop<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.ListRightPopAsync(key);
            return ToObject<T>(ret);
        }

        public async Task<T> ListRightPopLeftPush<T>(string source, string destination)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(destination, nameof(destination));
            source = AddKeyPrefix(source);
            destination = AddKeyPrefix(destination);
            var ret = await _db.ListRightPopLeftPushAsync(source, destination);
            return ToObject<T>(ret);
        }

        public async Task<long> ListRightPush<T>(string key, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.ListRightPushAsync(key, ToRedisValue(value));
        }

        public async Task<long> ListRightPush<T>(string key, List<T> values)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = ToRedisValues(values);
            return await _db.ListRightPushAsync(key, ret);
        }

        public async Task ListSetByIndex<T>(string key, long index, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            await _db.ListSetByIndexAsync(key, index, ToRedisValue(value));
        }

        public async Task ListTrim(string key, long start, long end)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            await _db.ListTrimAsync(key, start, end);
        }
        #endregion

        #region Set 集合
        public async Task<bool> SetAdd<T>(string key, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SetAddAsync(key, ToRedisValue(value));
        }

        public async Task<long> SetAdd<T>(string key, List<T> values)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = ToRedisValues(values);
            return await _db.SetAddAsync(key, ret);
        }

        public async Task<List<T>> SetCombine<T>(SetOperation operation, string first, string second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            first = AddKeyPrefix(first);
            second = AddKeyPrefix(second);
            var ret = await _db.SetCombineAsync(operation, first, second);
            return ToObjects<T>(ret);
        }

        public async Task<List<T>> SetCombine<T>(SetOperation operation, List<string> keys)
        {
            Check.NotListNull(keys, nameof(keys));
            var rkeys = ToRedisKeys(keys);
            var ret = await _db.SetCombineAsync(operation, rkeys);
            return ToObjects<T>(ret);
        }

        public async Task<long> SetCombineAndStore(SetOperation operation, string destination, string first, string second)
        {
            Check.NotNull(destination, nameof(destination));
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            return await _db.SetCombineAndStoreAsync(operation, destination, first, second);
        }

        public async Task<long> SetCombineAndStore(SetOperation operation, string destination, List<string> keys)
        {
            Check.NotNull(destination, nameof(destination));
            Check.NotListNull(keys, nameof(keys));
            var rkeys = ToRedisKeys(keys);
            return await _db.SetCombineAndStoreAsync(operation, destination, rkeys);
        }

        public async Task<bool> SetContains<T>(string key, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SetContainsAsync(key, ToRedisValue(value));
        }

        public async Task<long> SetLength(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SetLengthAsync(key);
        }

        public async Task<List<T>> SetMembers<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SetMembersAsync(key);
            return ToObjects<T>(ret);
        }

        public async Task<bool> SetMove<T>(string source, string destination, T value)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(destination, nameof(destination));
            source = AddKeyPrefix(source);
            destination = AddKeyPrefix(destination);
            return await _db.SetMoveAsync(source, destination, ToRedisValue(value));
        }

        public async Task<T> SetPop<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SetPopAsync(key);
            return ToObject<T>(ret);
        }

        public async Task<List<T>> SetPop<T>(string key, long count)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SetPopAsync(key, count);
            return ToObjects<T>(ret);
        }

        public async Task<T> SetRandomMember<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SetRandomMemberAsync(key);
            return ToObject<T>(ret);
        }

        public async Task<List<T>> SetRandomMembers<T>(string key, long count)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SetRandomMembersAsync(key, count);
            return ToObjects<T>(ret);
        }

        public async Task<bool> SetRemove<T>(string key, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SetRemoveAsync(key, ToRedisValue(value));
        }

        public async Task<long> SetRemove<T>(string key, List<T> values)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SetRemoveAsync(key, ToRedisValues(values));
        }

        public async Task<List<T>> SetScan<T>(string key, string pattern = default, int pageSize = 20, long cursor = 0L, int pageOffset = 0)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var enumerator = _db.SetScanAsync(key, pattern, pageSize, cursor, pageOffset).GetAsyncEnumerator();
            var ret = new List<T>();
            while(await enumerator.MoveNextAsync())
            {
                ret.Add(ToObject<T>(enumerator.Current));
            }
            return ret;
        }
        #endregion

        #region SortedSet 有序集合
        public async Task<bool> SortedSetAdd<T>(string key, T member, double score)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetAddAsync(key, ToRedisValue(member), score);
        }

        public async Task<double> SortedSetDecrement<T>(string key, T member, double value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetDecrementAsync(key, ToRedisValue(member), value);
        }

        public async Task<double> SortedSetIncrement<T>(string key, T member, double value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetIncrementAsync(key, ToRedisValue(member), value);
        }

        public async Task<long> SortedSetLength(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetLengthAsync(key);
        }

        public async Task<long> SortedSetLengthByValue<T>(string key, T min, T max)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetLengthByValueAsync(key, ToRedisValue(min), ToRedisValue(max));
        }

        public async Task<List<T>> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1, bool isAsc = true)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SortedSetRangeByRankAsync(key, start, stop, isAsc ? Order.Ascending : Order.Descending);
            return ToObjects<T>(ret);
        }

        public async Task<List<(T Element, double Score)>> SortedSetRangeByRankWithScores<T>(string key, long start = 0, long stop = -1, bool isAsc = true)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SortedSetRangeByRankWithScoresAsync(key, start, stop, isAsc ? Order.Ascending : Order.Descending);
            return ret.Select(s => (ToObject<T>(s.Element), s.Score)).ToList();
        }

        public async Task<List<T>> SortedSetRangeByScore<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, bool isAsc = true, long skip = 0, long take = -1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SortedSetRangeByScoreAsync(key, start, stop, Exclude.None, isAsc ? Order.Ascending : Order.Descending, skip, take);
            return ToObjects<T>(ret);
        }

        public async Task<List<(T Element, double score)>> SortedSetRangeByScoreWithScores<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, bool isAsc = true, long skip = 0, long take = -1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SortedSetRangeByScoreWithScoresAsync(key, start, stop, Exclude.None, isAsc ? Order.Ascending : Order.Descending, skip, take);
            return ret.Select(s => (ToObject<T>(s.Element), s.Score)).ToList();
        }

        public async Task<List<T>> SortedSetRangeByValue<T>(string key, T min, T max, bool isAsc = true, long skip = 0, long take = -1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.SortedSetRangeByValueAsync(key, ToRedisValue(min), ToRedisValue(max), Exclude.None, isAsc ? Order.Ascending : Order.Descending, skip, take);
            return ToObjects<T>(ret);
        }

        public async Task<long?> SortedSetRank<T>(string key, T member, bool isAsc = true)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetRankAsync(key, ToRedisValue(member), isAsc ? Order.Ascending : Order.Descending);
        }

        public async Task<bool> SortedSetRemove<T>(string key, T member)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetRemoveAsync(key, ToRedisValue(member));
        }

        public async Task<long> SortedSetRemove<T>(string key, List<T> members)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetRemoveAsync(key, ToRedisValues(members));
        }

        public async Task<long> SortedSetRemoveRangeByRank(string key, long start, long end)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetRemoveRangeByRankAsync(key, start, end);
        }

        public async Task<long> SortedSetRemoveRangeByScore(string key, double start, double end)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetRemoveRangeByScoreAsync(key, start, end);
        }

        public async Task<long> SortedSetRemoveRangeByValue<T>(string key, T min, T max)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetRemoveRangeByValueAsync(key, ToRedisValue(min), ToRedisValue(max));
        }

        public async Task<List<(T Element, double Score)>> SortedSetScan<T>(string key, string pattern, int pageSize = 20, long cursor = 0L, int pageOffset = 0)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var enumerator = _db.SortedSetScanAsync(key, pattern, pageSize, cursor, pageOffset).GetAsyncEnumerator();
            var ret = new List<(T Element, double Score)>();
            while (await enumerator.MoveNextAsync())
            {
                ret.Add((ToObject<T>(enumerator.Current.Element), enumerator.Current.Score));
            }
            return ret;
        }

        public async Task<double?> SortedSetScore<T>(string key, T member)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.SortedSetScoreAsync(key, ToRedisValue(member));
        }
        #endregion

        #region Lock 锁相关
        public async Task<bool> LockTake<T>(string key, T value, TimeSpan expiry)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.LockTakeAsync(key, ToRedisValue(value), expiry);
        }

        public async Task<bool> LockExtend<T>(string key, T value, TimeSpan expiry)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.LockExtendAsync(key, ToRedisValue(value), expiry);
        }

        public async Task<T> LockQuery<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.LockQueryAsync(key);
            return ToObject<T>(ret);
        }

        public async Task<bool> LockRelease<T>(string key, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.LockReleaseAsync(key, ToRedisValue(value));
        }
        #endregion

        #region Hash 哈希
        public async Task<long> HashDecrement(string key, string hashField, long value = 1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashDecrementAsync(key, hashField, value);
        }

        public async Task<double> HashDecrement(string key, string hashField, double value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashDecrementAsync(key, hashField, value);
        }

        public async Task<bool> HashDelete(string key, string hashField)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashDeleteAsync(key, hashField);
        }

        public async Task<long> HashDelete(string key, List<string> hashFields)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var fields = hashFields.Select(s => new RedisValue(s)).ToArray();
            return await _db.HashDeleteAsync(key, fields);
        }

        public async Task<bool> HashExists(string key, string hashField)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashExistsAsync(key, hashField);
        }

        public async Task<T> HashGet<T>(string key, string hashField)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.HashGetAsync(key, hashField);
            return ToObject<T>(ret);
        }

        public async Task<List<T>> HashGet<T>(string key, List<string> hashFields)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var fields = hashFields.Select(s => new RedisValue(s)).ToArray();
            var ret = await _db.HashGetAsync(key, fields);
            return ToObjects<T>(ret);
        }

        public async Task<List<(string Name, T Value)>> HashGetAll<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.HashGetAllAsync(key);
            return ret.Select(s => (s.Name.ToString(), ToObject<T>(s.Value))).ToList();
        }

        public async Task<long> HashIncrement(string key, string hashField, long value = 1)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashIncrementAsync(key, hashField, value);
        }

        public async Task<double> HashIncrementAsync(string key, string hashField, double value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashIncrementAsync(key, hashField, value);
        }

        public async Task<List<string>> HashKeys(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.HashKeysAsync(key);
            return ret.Select(s => s.ToString()).ToList();
        }

        public async Task<long> HashLength(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashLengthAsync(key);
        }

        public async Task HashSet<T>(string key, List<KeyValuePair<string, T>> values)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var item = values.Select(s => new HashEntry(s.Key, ToRedisValue(s.Value))).ToArray();
            await _db.HashSetAsync(key, item);
        }

        public async Task<bool> HashSet<T>(string key, string hashField, T value)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            return await _db.HashSetAsync(key, hashField, ToRedisValue(value));
        }

        public async Task<List<T>> HashValues<T>(string key)
        {
            Check.NotNull(key, nameof(key));
            key = AddKeyPrefix(key);
            var ret = await _db.HashValuesAsync(key);
            return ToObjects<T>(ret);
        }
        #endregion

        #region Subscribe 订阅
        public async Task<long> Publish<T>(string channel, T message)
        {
            return await _db.PublishAsync(channel, ToRedisValue(message));
        }

        public async Task Subscribe<T>(string channel, Action<string, T> handler)
        {
            await _connection.Value.GetSubscriber().SubscribeAsync(channel, (c, value) =>
            {
                handler?.Invoke(channel, ToObject<T>(value));
            });
        }

        public async Task Unsubscribe<T>(string channel, Action<string, T> handler = null)
        {
            await _connection.Value.GetSubscriber().UnsubscribeAsync(channel, (c, value) =>
            {
                handler?.Invoke(channel, ToObject<T>(value));
            });
        }

        public async Task UnsubscribeAll()
        {
            await _connection.Value.GetSubscriber().UnsubscribeAllAsync();
        } 
        #endregion

        #region Transaction 事务
        public ITransaction BeginTransaction()
        {
            return _db.CreateTransaction();
        }

        public IDatabase GetDatabase()
        {
            return _db;
        } 
        #endregion

        #region 辅助方法
        /// <summary>
        /// 添加key前缀
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string AddKeyPrefix(string key)
        {
            return string.IsNullOrWhiteSpace(_option.KeyPrefix) ? key : $"{_option.KeyPrefix}:{key}";
        }


        /// <summary>
        /// 把键列表换成RedisKey，会加上前缀
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private RedisKey[] ToRedisKeys(IEnumerable<string> keys)
        {
            return keys.ToList().Select(s => new RedisKey(AddKeyPrefix(s))).ToArray();
        }

        /// <summary>
        /// 转成ReidsKey和RedisValue集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        private KeyValuePair<RedisKey, RedisValue>[] ToRedisKeyAndRedisValue<T>(List<KeyValuePair<string, T>> values)
        {
            return values.Select(s =>
                     new KeyValuePair<RedisKey, RedisValue>(new RedisKey(AddKeyPrefix(s.Key)), ToRedisValue(s.Value)))
                .ToArray();
        }

        /// <summary>
        /// 转成ReidsValue数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        private RedisValue[] ToRedisValues<T>(List<T> values)
        {
            return values.Select(s => ToRedisValue(s)).ToArray();
        }

        /// <summary>
        /// 转成成RedisValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private RedisValue ToRedisValue<T>(T value)
        {
            return new RedisValue(JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 把RedisValue值转成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private T ToObject<T>(RedisValue value)
        {
            if (!value.HasValue)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(value);
        } 


        /// <summary>
        /// RedisValue集合转成对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        private List<T> ToObjects<T>(RedisValue[] values)
        {
            return values.Select(s => ToObject<T>(s)).ToList();
        }
        #endregion
    }
}
