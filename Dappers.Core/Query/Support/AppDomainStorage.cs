using System.Collections.Generic;

namespace Dappers.Query.Support
{

    public class AppComainStorage : ILocalStorage
    {
        IDictionary<string, object> CallContext = new Dictionary<string, object>();
        
        public object GetData(string name)
        {
            object val=null;
            CallContext.TryGetValue(name, out val);
            return val;
        }


        public void SetData(string name, object value)
        {
            CallContext[name] = value;
        }


        public void FreeNamedDataSlot(string name)
        {
            CallContext.Clear();
        }
    }
}