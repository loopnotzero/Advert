using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdsTopicPreviewException : Exception
    {
        public AdsTopicPreviewException()
        {
        }

        protected AdsTopicPreviewException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdsTopicPreviewException(string message) : base(message)
        {
        }

        public AdsTopicPreviewException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}