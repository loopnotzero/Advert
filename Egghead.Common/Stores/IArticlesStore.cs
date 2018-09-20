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
        Task<T> FindArticleByIdOrDefaultAsync(ObjectId articleId, T defaultValue, CancellationToken cancellationToken);    
        Task<T> FindArticleByNormalizedTitleAsync(string title, CancellationToken cancellationToken);
        Task<T> FindArticleByNormalizedTitleOrDefaultAsync(string title, T defaultValue, CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(int howManyElements, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByIdAsync(ObjectId articleId, T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleViewsCountById(ObjectId articleId, double viewsCount, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleByNormalizedTitleAsync(string title, T entity, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken);      
        Task<OperationResult> DeleteArticleByNormalizedTitleAsync(string title, CancellationToken cancellationToken);
    }
}