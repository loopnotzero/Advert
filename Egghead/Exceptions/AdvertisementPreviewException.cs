using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdvertisementPreviewException : Exception
    {
        public AdvertisementPreviewException()
        {
        }

        protected AdvertisementPreviewException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdvertisementPreviewException(string message) : base(message)
        {
        }

        public AdvertisementPreviewException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}