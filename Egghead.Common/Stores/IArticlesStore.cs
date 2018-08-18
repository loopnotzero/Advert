using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesStore<T> : IDisposable where T : class
    {              
        Task SetNormalizedTitleAsync(T entity, string normalizedTitle, CancellationToken cancellationToken);
        Task<T> FindArticleByIdAsync(string articleId, CancellationToken cancellationToken);
        Task<T> FindArticleByTitleAsync(string title, CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(int limit, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByIdAsync(string articleId, T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByTitleAsync(string articleTitle, T entity, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleByIdAsync(string articleId, CancellationToken cancellationToken);      
        Task<OperationResult> DeleteArticleByTitleAsync(string articleTitle, CancellationToken cancellationToken);
    }
}