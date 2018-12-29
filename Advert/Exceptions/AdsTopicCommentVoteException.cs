using System;
using System.Runtime.Serialization;

namespace Advert.Exceptions
{
    public class AdsTopicCommentVoteException : Exception
    {
        public AdsTopicCommentVoteException()
        {
        }

        protected AdsTopicCommentVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AdsTopicCommentVoteException(string message) : base(message)
        {
        }

        public AdsTopicCommentVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}