using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dappers
{
    public class DappersException : Exception
    {
        public DappersException(string message, Exception cause)
        : base(message, cause)
          {
          }

        public DappersException(string message)
            : base(message)
        {
        }
    }
}
