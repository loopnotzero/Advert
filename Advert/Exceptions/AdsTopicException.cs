using System;
using System.Runtime.Serialization;

namespace Advert.Exceptions
{
    public class AdsTopicException : Exception
    {
        public AdsTopicException()
        {
        }

        protected AdsTopicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdsTopicException(string message) : base(message)
        {
        }

        public AdsTopicException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}