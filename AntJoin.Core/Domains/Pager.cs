using System;

namespace AntJoin.Core.Domains
{
    /// <summary>
    /// 分页参数
    /// </summary>
    [Serializable]
    public class Pager
    {
        private int _pageIndex;


        /// <summary>
        /// 初始化分页参数
        /// </summary>
        public Pager() : this(1)
        {

        }


        /// <summary>
        /// 初始化分页参数
        /// </summary>
        /// <param name="page">页索引</param>
        /// <param name="size">每页显示行数,默认20</param>
        /// <param name="order">排序条件</param>
        public Pager(int page, int size, string order) : this(page, size, 0, order)
        {

        }


        /// <summary>
        /// 初始化分页参数
        /// </summary>
        /// <param name="page">页索引</param>
        /// <param name="size">每页显示行数,默认20</param>
        /// <param name="total">总行数</param>
        /// <param name="order">排序条件</param>
        public Pager(int page, int size = 20, int total = 0, string order = "Id Desc")
        {
            Page = page;
            Size = size;
            Total = total;
            Order = order;
        }

        
        /// <summary>
        /// 页索引，即第几页，从1开始
        /// </summary>
        public virtual int Page
        {
            get
            {
                if (_pageIndex <= 0)
                {
                    _pageIndex = 1;
                }
                return _pageIndex;
            }
            set => _pageIndex = value;
        }


        /// <summary>
        /// 每页显示行数
        /// </summary>
        public virtual int Size { get; set; }


        /// <summary>
        /// 总行数
        /// </summary>
        public virtual int Total { get; set; }


        /// <summary>
        /// 排序条件
        /// </summary>
        public virtual string Order { get; set; }


        /// <summary>
        /// 获取跳过的行数
        /// </summary>
        /// <returns></returns>
        public int GetSkipCount()
        { 
            if (Page > GetPageCount())
            {
                Page = GetPageCount();
            }
            return Size * (Page - 1);
        }


        /// <summary>
        /// 起始行数
        /// </summary>
        /// <returns></returns>
        public int GetStartNumber() => (Page - 1) * Size + 1;


        /// <summary>
        /// 结束行数
        /// </summary>
        /// <returns></returns>
        public int GetEndNumber() => Page * Size;


        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <returns></returns>
        private int GetPageCount() => Total % Size == 0 ?
                Total / Size :
                Total / Size + 1;
    }
}
