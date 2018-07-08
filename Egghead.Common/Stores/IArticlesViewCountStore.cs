using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesViewCountStore<T> : IDisposable where T : class
    {
        Task<T> FindArticlesViewCountByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<long> CountArticlesViewCountByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<OperationResult> AddArticleViewAsync(T entity , CancellationToken cancellationToken);
    }
}