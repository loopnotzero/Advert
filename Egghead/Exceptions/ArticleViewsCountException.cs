using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ArticleViewsCountException : Exception
    {
        public ArticleViewsCountException()
        {
        }

        protected ArticleViewsCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArticleViewsCountException(string message) : base(message)
        {
        }

        public ArticleViewsCountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}