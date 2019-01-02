using System;
using System.Runtime.Serialization;

namespace Advert.Exceptions
{
    public class PostCommentException : Exception
    {
        public PostCommentException()
        {
        }

        protected PostCommentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PostCommentException(string message) : base(message)
        {
        }

        public PostCommentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}