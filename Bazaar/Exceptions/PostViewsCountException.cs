using System;
using System.Runtime.Serialization;

namespace Bazaar.Exceptions
{
    public class PostViewsCountException : Exception
    {
        public PostViewsCountException()
        {
        }

        protected PostViewsCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PostViewsCountException(string message) : base(message)
        {
        }

        public PostViewsCountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}