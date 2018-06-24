using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Egghead.MongoDbStorage.IStores
{
    public interface IArticleStore<T> : IDisposable where T : class
    {              
        Task SetNormalizedTitleAsync(T article, string normalizedTitle, CancellationToken cancellationToken);
        Task<T> FindArticleByIdAsync(string articleId, CancellationToken cancellationToken);
        Task<T> FindArticleByTitleAsync(string normalizedTitle, CancellationToken cancellationToken);
        Task<List<T>> GetArticles(CancellationToken cancellationToken);
        Task<IdentityResult> CreateArticleAsync(T article, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteArticleAsync(T article, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateArticleAsync(T article, CancellationToken cancellationToken);
    }
}