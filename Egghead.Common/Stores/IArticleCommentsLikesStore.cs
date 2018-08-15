using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    public interface IArticleCommentsLikesStore<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentLikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken);
        Task<T> FindArticleCommentDislikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken);
        Task<long> CountArticleCommentLikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken);
        Task<long> CountArticleCommentDislikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken);
        Task<OperationResult> SetArticleCommentLikeAsync(T entity, CancellationToken cancellationToken);
    }
}