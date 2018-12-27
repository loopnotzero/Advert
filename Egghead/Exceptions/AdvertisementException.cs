using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdvertisementException : Exception
    {
        public AdvertisementException()
        {
        }

        protected AdvertisementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdvertisementException(string message) : base(message)
        {
        }

        public AdvertisementException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}