using System;
using System.Runtime.Serialization;

namespace Bazaar.Exceptions
{
    public class PostCommentVoteException : Exception
    {
        public PostCommentVoteException()
        {
        }

        protected PostCommentVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PostCommentVoteException(string message) : base(message)
        {
        }

        public PostCommentVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}