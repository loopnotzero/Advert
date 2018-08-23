using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ProfileInformationException : Exception
    {
        public ProfileInformationException()
        {
        }

        protected ProfileInformationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ProfileInformationException(string message) : base(message)
        {
        }

        public ProfileInformationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}