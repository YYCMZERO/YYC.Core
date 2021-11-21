using System;
using System.Collections.Generic;
using System.Linq;

namespace AntJoin.Redis
{
    internal class Check
    {
        internal static void NotNull(string value, string parameterName)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException($"参数 {parameterName} 不能为空");
            }
        }


        internal static void NotListNull<T>(IEnumerable<T> items, string parameterName)
        {
            if(items == null || !items.Any())
            {
                throw new ArgumentNullException($"集合参数 {parameterName} 不能为空或者空集合");
            }
        }


        internal static void NotNull<T>(T item, string parameterName)
        {
            if(item == null)
            {
                throw new ArgumentNullException($"实体参数 {parameterName} 不能为空");
            }
        }
    }
}
