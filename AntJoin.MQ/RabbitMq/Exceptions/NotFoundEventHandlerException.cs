using System;

namespace Microsoft.Extensions.DependencyInjection.RabbitMq.Exceptions
{
    public class NotFoundEventHandlerException : Exception
    {
        public NotFoundEventHandlerException(string message) : base(message)
        {
            
        }
    }
}