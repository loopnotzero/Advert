using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ArticleCommentVoteException : Exception
    {
        public ArticleCommentVoteException()
        {
        }

        protected ArticleCommentVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArticleCommentVoteException(string message) : base(message)
        {
        }

        public ArticleCommentVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}