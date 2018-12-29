using System;
using System.Runtime.Serialization;

namespace Advert.Exceptions
{
    public class AdsTopicCommentException : Exception
    {
        public AdsTopicCommentException()
        {
        }

        protected AdsTopicCommentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdsTopicCommentException(string message) : base(message)
        {
        }

        public AdsTopicCommentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}