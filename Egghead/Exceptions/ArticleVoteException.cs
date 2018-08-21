using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ArticleVoteException : Exception
    {
        public ArticleVoteException()
        {
        }

        protected ArticleVoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArticleVoteException(string message) : base(message)
        {
        }

        public ArticleVoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}