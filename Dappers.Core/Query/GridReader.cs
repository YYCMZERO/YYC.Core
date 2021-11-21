using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace Dappers
{
    /// <summary>
    /// 多结果集读取器
    /// 保留对reader的引用,仅connection未关闭时可用.
    /// </summary>
    public class GridReader
    {
        private Dapper.SqlMapper.GridReader reader { get; set; }
        public GridReader(Dapper.SqlMapper.GridReader r,Dapper.DynamicParameters ps)
        {
            this.reader = r;
            this.OutputParams = ps;
        }

        /// <summary>
        /// 应立即读取数据 如: Read().ToList() 否则导致读取异常; 对多个resultset,多次调用此方法,但每个resultset仅可读取一次
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Read<T>()
        {
            return reader.Read<T>();
        }

        /// <summary>
        /// 分隔符为Id, 则结果中o1,o2都必须包括Id
        /// </summary>
        /// <typeparam name="TFirst">对象1</typeparam>
        /// <typeparam name="TSecond">对象2</typeparam>
        /// <typeparam name="TReturn">返回的对象类型</typeparam>
        /// <param name="func">对象关联关系的委托如(o1,o2)=>{o1.Child=o2,return o1}</param>
        /// <param name="splitOn">select语句两个对象分前后段落,分隔的标记字段.如id</param>
        /// <returns></returns>
        public IEnumerable<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn)
        {
            return reader.Read<TFirst, TSecond, TReturn>(func,splitOn);
        }
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn)
        {
            return reader.Read<TFirst, TSecond,TThird, TReturn>(func,splitOn);
        }
        public IEnumerable<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn)
        {
            return reader.Read<TFirst, TSecond, TThird, TFourth, TReturn>(func, splitOn);
        }

        /// <summary>
        /// 仅在所有结果集遍历完,方可读取值.
        /// </summary>
        public Dapper.DynamicParameters OutputParams
        {
            get;
            set;
        }
    }

}
