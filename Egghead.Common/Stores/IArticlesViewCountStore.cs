using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesViewCountStore<in T> : IDisposable where T : class
    {
        Task<long> CountArticleViewsCountByArticleIdAsync(string articleId, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleViewsCountAsync(T entity , CancellationToken cancellationToken);
        Task<IEnumerable<string>> FindArticlesPopularOnEgghead(int limit, CancellationToken cancellationToken);
    }
}