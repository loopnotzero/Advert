using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesViewsStore<T> : IDisposable where T : class
    {
        Task<T> FindArticlesViewsByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticlesViewsByArticleIdAsync(string articleId, CancellationToken cancellationToken);
    }
}