using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesCommentsStore<T> : IDisposable where T : class
    {
        Task<long> CountArticlesCommentsByArticleIdAsync(string id, CancellationToken cancellationToken);
    }
}