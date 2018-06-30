using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesStore<T> : IDisposable where T : class
    {              
        Task SetNormalizedTitleAsync(T article, string normalizedTitle, CancellationToken cancellationToken);
        Task<T> FindArticleByIdAsync(string objectId, CancellationToken cancellationToken);
        Task<T> FindArticleByTitleAsync(string title, CancellationToken cancellationToken);
        Task<List<T>> GetArticles(CancellationToken cancellationToken);
        Task<IdentityResult> CreateArticleAsync(T article, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateArticleByIdAsync(string objectId, T article, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateArticleByTitleAsync(string title, T article, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteArticleByIdAsync(string objectId, CancellationToken cancellationToken);      
        Task<IdentityResult> DeleteArticleByTitleAsync(string title, CancellationToken cancellationToken);       
    }
}