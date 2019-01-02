using System;
using System.Runtime.Serialization;

namespace Advert.Exceptions
{
    public class PostException : Exception
    {
        public PostException()
        {
        }

        protected PostException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PostException(string message) : base(message)
        {
        }

        public PostException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}