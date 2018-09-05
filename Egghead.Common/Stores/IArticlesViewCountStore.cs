using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesViewCountStore<T> : IDisposable where T : class
    {
        Task<long> CountArticleViewsCountByArticleIdAsync(ObjectId articleId, CancellationToken cancellationToken);
        Task<IQueryable<T>> AsQueryable(CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleViewsCountAsync(T entity , CancellationToken cancellationToken);
    }
}