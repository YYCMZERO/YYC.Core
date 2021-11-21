using System;
using System.Collections.Generic;
using System.Text;

namespace AntJoin.Dapper
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {

        }
    }
}
