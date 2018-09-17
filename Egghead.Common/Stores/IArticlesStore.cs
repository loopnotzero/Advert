using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesStore<T> : IDisposable where T : class
    {              
        Task SetNormalizedTitleAsync(T entity, string normalizedTitle, CancellationToken cancellationToken);
        Task<T> FindArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken);
        Task<T> FindArticleByNormalizedTitleAsync(string title, CancellationToken cancellationToken);
        Task<long> CountArticlesByNormalizedEmailAsync(string email, CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(int limit, CancellationToken cancellationToken);
        Task<List<T>> FindRecentArticlesByNormalizedEmailAsync(string email, int limit, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByIdAsync(ObjectId articleId, T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByTitleAsync(string title, T entity, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken);      
        Task<OperationResult> DeleteArticleByTitleAsync(string title, CancellationToken cancellationToken);
    }
}