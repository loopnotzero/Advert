using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Entities;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesLikesStore<T> : IDisposable where T : class
    {        
        Task<T> FindArticlesLikesByArticleIdAsync(string id, CancellationToken cancellationToken);
        Task<T> FindArticlesUnlikesByArticleIdAsync(string id, CancellationToken cancellationToken);
        Task<long> CountArticlesLikesByArticleIdAsync(string id, CancellationToken cancellationToken);
        Task<long> CountArticlesUnlikesByArticleIdAsync(string id, CancellationToken cancellationToken);
        Task<OperationResult> AddLikeAsync(T entity , CancellationToken cancellationToken);
    }
}