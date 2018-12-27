using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdvertisementVoteException : Exception
    {
        public AdvertisementVoteException()
        {
        }

        protected AdvertisementVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdvertisementVoteException(string message) : base(message)
        {
        }

        public AdvertisementVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}