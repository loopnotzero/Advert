using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdsTopicVoteException : Exception
    {
        public AdsTopicVoteException()
        {
        }

        protected AdsTopicVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdsTopicVoteException(string message) : base(message)
        {
        }

        public AdsTopicVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}