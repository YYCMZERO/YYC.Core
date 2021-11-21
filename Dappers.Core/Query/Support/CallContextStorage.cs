//copy from Spring.core: Spring.Threading

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

namespace Dappers.Query.Support
{
    /// <author>Erich Eichinger</author>
    public class CallContextStorage : ILocalStorage
    {
        private static ThreadLocal<Dictionary<string, object>> callContextDic = new ThreadLocal<Dictionary<string, object>>(()=>new Dictionary<string, object> { });

        /// <summary>
        /// Retrieves an object with the specified name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>The object in the call context associated with the specified name or null if no object has been stored previously</returns>
        public object GetData(string name)    
        {
            if (callContextDic.Value.ContainsKey(name))
            {
                var obj = callContextDic.Value[name];
                return obj;
            }
            else
                return null;
        }

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item.</param>
        /// <param name="value">The object to store in the call context.</param>
        public void SetData(string name, object value)
        {
            callContextDic.Value.Add(name, value);
        }

        /// <summary>
        /// Empties a data slot with the specified name.
        /// </summary>
        /// <param name="name">The name of the data slot to empty.</param>
        public void FreeNamedDataSlot(string name)
        {
            callContextDic.Value.Remove(name);
        }
    }
}