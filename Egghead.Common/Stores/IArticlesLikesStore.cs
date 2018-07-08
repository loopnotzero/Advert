using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesLikesStore<T> : IDisposable where T : class
    {        
        Task<T> FindArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<T> FindArticlesDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticlesDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<OperationResult> AddArticleLikeAsync(T entity , CancellationToken cancellationToken);
    }
}