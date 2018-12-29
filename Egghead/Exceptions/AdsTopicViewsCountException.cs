using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdsTopicViewsCountException : Exception
    {
        public AdsTopicViewsCountException()
        {
        }

        protected AdsTopicViewsCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdsTopicViewsCountException(string message) : base(message)
        {
        }

        public AdsTopicViewsCountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}