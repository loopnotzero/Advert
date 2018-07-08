using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesLikesStore<T> : IDisposable where T : class
    {        
        Task<T> FindArticleLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<T> FindArticleDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticleLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticleDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<OperationResult> AddArticleLikeAsync(T entity , CancellationToken cancellationToken);
    }
}