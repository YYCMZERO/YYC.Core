using System;
using System.Collections.Generic;
using System.Text;

namespace Dappers
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {

        }
    }
}
