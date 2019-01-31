using System;
using System.Runtime.Serialization;

namespace Bazaar.Exceptions
{
    public class PostVoteException : Exception
    {
        public PostVoteException()
        {
        }

        protected PostVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PostVoteException(string message) : base(message)
        {
        }

        public PostVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}