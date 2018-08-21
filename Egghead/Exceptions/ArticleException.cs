using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ArticleException : Exception
    {
        public ArticleException()
        {
        }

        protected ArticleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArticleException(string message) : base(message)
        {
        }

        public ArticleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}