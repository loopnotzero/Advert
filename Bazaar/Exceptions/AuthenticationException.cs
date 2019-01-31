using System;
using System.Runtime.Serialization;

namespace Bazaar.Exceptions
{
    public class AccountException : Exception
    {
        public AccountException()
        {
        }

        protected AccountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AccountException(string message) : base(message)
        {
        }

        public AccountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}