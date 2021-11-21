using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntJoin.Core.Domains
{
    /// <summary>
    /// 分页集合
    /// </summary>
    [Serializable]
    public class PagingResult : PagingResult<object>
    {
        /// <summary>
        /// 初始化分页集合
        /// </summary>
        public PagingResult() : this(0)
        {
        }


        /// <summary>
        /// 初始化分页集合
        /// </summary>
        /// <param name="rows">行数据集合</param>
        public PagingResult(IEnumerable<object> rows = null) : this(0, rows)
        {
        }


        /// <summary>
        /// 初始化分页集合
        /// </summary>
        /// <param name="total">总行数</param>
        /// <param name="rows">行数据集合</param>
        public PagingResult(int total, IEnumerable<object> rows = null) : this(1, 20, total, rows)
        {
        }


        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页行数</param>
        /// <param name="total">总行数</param>
        /// <param name="rows">行数据集合</param>
        public PagingResult(int page, int size, int total, IEnumerable<object> rows = null) : base(page, size, total, rows)
        {

        }
    }





    /// <summary>
    /// 分页集合
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [Serializable]
    public class PagingResult<T>
    {
        /// <summary>
        /// 初始化分页集合
        /// </summary>
        public PagingResult() : this(0)
        {
        }


        /// <summary>
        /// 初始化分页集合
        /// </summary>
        /// <param name="rows">行数据集合</param>
        public PagingResult(IEnumerable<T> rows = null) : this(0, rows)
        {
        }


        /// <summary>
        /// 初始化分页集合
        /// </summary>
        /// <param name="total">总行数</param>
        /// <param name="rows">行数据集合</param>
        public PagingResult(int total, IEnumerable<T> rows = null) : this(1, 20, total, rows)
        {
        }


        /// <summary>
        /// 初始化分页集合
        /// </summary>
        /// <param name="page">页索引</param>
        /// <param name="size">每页显示行数</param>
        /// <param name="total">总行数</param>
        /// <param name="rows">行数据集合</param>
        public PagingResult(int page, int size, int total, IEnumerable<T> rows = null)
        {
            Total = total;
            Page = page;
            Size = size;
            Rows = rows.ToList();
        }


        /// <summary>
        /// 页索引，即第几页，从1开始
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }


        /// <summary>
        /// 每页显示行数
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }


        /// <summary>
        /// 总行数
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }


        /// <summary>
        /// 总页数
        /// </summary>
        [JsonProperty("pages")]
        public int Pages => Total % Size == 0 ?
                Total / Size :
                Total / Size + 1;


        /// <summary>
        /// 数据行
        /// </summary>
        [JsonProperty("rows")]
        public List<T> Rows { get; set; }
    }
}
