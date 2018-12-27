using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdvertisementCommentException : Exception
    {
        public AdvertisementCommentException()
        {
        }

        protected AdvertisementCommentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdvertisementCommentException(string message) : base(message)
        {
        }

        public AdvertisementCommentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}