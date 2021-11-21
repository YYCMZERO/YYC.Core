using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dappers.Query.Support
{
    public interface ILocalStorage
    {
        object GetData(string name);
        void SetData(string name, object value);
        void FreeNamedDataSlot(string name);
    }
}
