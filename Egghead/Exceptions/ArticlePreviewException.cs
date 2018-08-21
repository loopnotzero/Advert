using System;
using System.Runtime.Serialization;

namespace Egghead.Exceptions
{
    public class ArticlePreviewException : Exception
    {
        public ArticlePreviewException()
        {
        }

        protected ArticlePreviewException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArticlePreviewException(string message) : base(message)
        {
        }

        public ArticlePreviewException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}