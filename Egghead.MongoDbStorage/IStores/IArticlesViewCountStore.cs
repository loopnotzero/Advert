using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesViewCountStore<T> : IDisposable where T : class
    {
        Task<T> FindArticlesViewCountByArticleIdAsync(string id, CancellationToken cancellationToken);
        Task<long> CountArticlesViewCountByArticleIdAsync(string id, CancellationToken cancellationToken);
    }
}