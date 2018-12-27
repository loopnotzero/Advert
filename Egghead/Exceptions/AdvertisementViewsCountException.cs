using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdvertisementViewsCountException : Exception
    {
        public AdvertisementViewsCountException()
        {
        }

        protected AdvertisementViewsCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdvertisementViewsCountException(string message) : base(message)
        {
        }

        public AdvertisementViewsCountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}