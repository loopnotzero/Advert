using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesViewCountStore<T> : IDisposable where T : class
    {
        Task<T> FindArticleViewCountByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticleViewCountByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<OperationResult> SetArticleViewCountAsync(T entity , CancellationToken cancellationToken);
    }
}