using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ProfileDescriptionException : Exception
    {
        public ProfileDescriptionException()
        {
        }

        protected ProfileDescriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ProfileDescriptionException(string message) : base(message)
        {
        }

        public ProfileDescriptionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}