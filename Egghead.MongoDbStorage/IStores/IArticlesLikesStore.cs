using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesLikesStore<T> : IDisposable where T : class
    {
        Task<T> FindArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<T> FindArticlesUnlikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticlesUnlikesByArticleIdAsync(string articleId, CancellationToken cancellationToken);
    }
}