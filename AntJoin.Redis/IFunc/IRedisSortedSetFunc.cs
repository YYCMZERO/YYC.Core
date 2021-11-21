using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Redis
{
    /// <summary>
    /// 有序集合(SortedSet)的相关操作
    /// </summary>
    public interface IRedisSortedSetFunc
    {
        // <summary>
        /// 新增一条数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="score">分数</param>
        /// <returns></returns>
        Task<bool> SortedSetAdd<T>(string key, T member, double score);

        /// <summary>
        /// 对Score值自减
        /// 如果不存在这member值，则执行增加member操作，并返回当前Score值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<double> SortedSetDecrement<T>(string key, T member, double value);

        /// <summary>
        /// 对Score值自增
        /// 如果不存在这member值，则执行增加member操作，并返回当前Score值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<double> SortedSetIncrement<T>(string key, T member, double value);

        /// <summary>
        /// 获取指定键的全部记录的数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<long> SortedSetLength(string key);

        /// <summary>
        ///  获取指定起始值到结束值的集合数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="min">起始值</param>
        /// <param name="max">结束值</param>
        /// <returns></returns>
        Task<long> SortedSetLengthByValue<T>(string key, T min, T max);

        /// <summary>
        /// 获取从 start 开始的 stop 条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByRank<T>(string key, long start = 0L, long stop = -1L, bool isAsc = true);

        /// <summary>
        /// 获取从 start 开始的 stop 条数据，同时还包含分数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="end">-1表示到结束，0为1条</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        Task<List<(T Element,double Score)>> SortedSetRangeByRankWithScores<T>(string key, long start = 0L, long stop = -1L, bool isAsc = true);

        /// <summary>
        /// 返回有序集合按分值 升序/降序 第 start 到 stop 个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start">起始值</param>
        /// <param name="stop">结束值</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="skip">跳过几条</param>
        /// <param name="take">获取几条</param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByScore<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, bool isAsc = true, long skip = 0L, long take = -1L);

        /// <summary>
        /// 返回有序集合按分值 升序/降序 第 start 到 stop 个元素，同时会带上分值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start">起始值</param>
        /// <param name="stop">结束值</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="skip">跳过几条</param>
        /// <param name="take">获取几条</param>
        /// <returns></returns>
        Task<List<(T Element,double score)>> SortedSetRangeByScoreWithScores<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, bool isAsc = true, long skip = 0L, long take = -1L);

        /// <summary>
        /// 返回有序集合按元素值 升序/降序 第 min 到 max 个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="min">起始值</param>
        /// <param name="max">结束值</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="skip">跳过几条</param>
        /// <param name="take">获取几条</param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByValue<T>(string key, T min, T max, bool isAsc = true, long skip = 0L, long take = -1L);

        /// <summary>
        /// 获取榜单某个元素的排名、排行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="isAsc">降序还是升序，默认升序</param>
        /// <returns></returns>
        Task<long?> SortedSetRank<T>(string key, T member, bool isAsc = true);

        /// <summary>
        /// 移除一条记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> SortedSetRemove<T>(string key, T member);

        /// <summary>
        /// 移除多条记录
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<long> SortedSetRemove<T>(string key, List<T> members);

        /// <summary>
        /// 移除有序集合中的 按排名排序 的第 start 到 end 的数据
        /// </summary>
        /// <param name="key">移除的key</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns></returns>
        Task<long> SortedSetRemoveRangeByRank(string key, long start, long end);

        /// <summary>
        /// 移除有序集合中的 按分值排序 的第 start 到 end 的数据
        /// </summary>
        /// <param name="key">移除的key</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns></returns>
        Task<long> SortedSetRemoveRangeByScore(string key, double start, double end);

        /// <summary>
        /// 删除指定起始值到结束值的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="min">起始值</param>
        /// <param name="max">结束值</param>
        /// <returns></returns>
        Task<long> SortedSetRemoveRangeByValue<T>(string key, T min, T max);


        /// <summary>
        /// 获取有序集合中的某个元素的值/分值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        Task<double?> SortedSetScore<T>(string key, T member);

        /// <summary>
        /// 模糊查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pattern">查找表达式</param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        Task<List<(T Element, double Score)>> SortedSetScan<T>(string key, string pattern, int pageSize = 20, long cursor = 0L, int pageOffset = 0);
    }
}
