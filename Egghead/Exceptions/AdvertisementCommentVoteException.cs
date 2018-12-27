using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class AdvertisementCommentVoteException : Exception
    {
        public AdvertisementCommentVoteException()
        {
        }

        protected AdvertisementCommentVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdvertisementCommentVoteException(string message) : base(message)
        {
        }

        public AdvertisementCommentVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}