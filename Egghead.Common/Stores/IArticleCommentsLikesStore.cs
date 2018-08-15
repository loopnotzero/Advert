using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    public interface IArticleCommentsLikesStore<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<T> FindArticleCommentDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticleCommentLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticleCommentDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<OperationResult> SetArticleCommentLikeAsync(T entity , CancellationToken cancellationToken);
    }
}