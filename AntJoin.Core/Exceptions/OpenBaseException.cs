using System;

namespace AntJoin.Core.Exceptions
{
    public class OpenBaseException : Exception
    {
        public OpenBaseException()
            : base()
        { }

        public OpenBaseException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public OpenBaseException(int errorCode, string errorMsg)
            : base(errorCode + ":" + errorMsg)
        {
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
        }

        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine + ErrorCode + ":" + ErrorMsg;
        }
    }
}
