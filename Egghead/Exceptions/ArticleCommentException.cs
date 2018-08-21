using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ArticleCommentException : Exception
    {
        public ArticleCommentException()
        {
        }

        protected ArticleCommentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArticleCommentException(string message) : base(message)
        {
        }

        public ArticleCommentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}