using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Microsoft.AspNetCore.Identity;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesStore<T> : IDisposable where T : class
    {              
        Task SetNormalizedTitleAsync(T entity, string normalizedTitle, CancellationToken cancellationToken);
        Task<T> FindArticleByIdAsync(string id, CancellationToken cancellationToken);
        Task<T> FindArticleByTitleAsync(string title, CancellationToken cancellationToken);
        Task<List<T>> GetArticles(CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByIdAsync(string id, T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByTitleAsync(string title, T entity, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleByIdAsync(string id, CancellationToken cancellationToken);      
        Task<OperationResult> DeleteArticleByTitleAsync(string title, CancellationToken cancellationToken);       
    }
}