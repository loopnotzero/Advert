using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesViewCountStore<T> : IDisposable where T : class
    {
        Task CreateArticleViewsCountAsync(T entity , CancellationToken cancellationToken);
        Task<T> FindArticleViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken);
        Task<long> CountArticleViewsCountByArticleIdAsync(ObjectId articleId, CancellationToken cancellationToken);
    }
}