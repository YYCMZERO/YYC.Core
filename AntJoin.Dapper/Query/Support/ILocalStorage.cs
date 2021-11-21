using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntJoin.Dapper.Query.Support
{
    public interface ILocalStorage
    {
        object GetData(string name);
        T GetData<T>(string name);
        void SetData(string name, object value);
        void FreeNamedDataSlot(string name);
    }
}
