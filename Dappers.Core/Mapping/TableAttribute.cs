using System;
using System.Collections.Generic;
using System.Text;

namespace Dappers.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableAttribute : Attribute
    {
        private bool flag = false;

        /// <summary>
        /// 表格名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否加入缓存读写
        /// </summary>
        public bool IsCacheReadWrite { get { return flag; } set { flag = value; } }

        public TableAttribute()
        {

        }

    }
}
