using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesStore<T> : IDisposable where T : class
    {      
        Task CreateArticleAsync(T entity, CancellationToken cancellationToken);
        Task SetNormalizedTitleAsync(T entity, string normalizedTitle, CancellationToken cancellationToken);      
        Task<T> FindArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken);
        Task<T> FindArticleByIdOrDefaultAsync(ObjectId articleId, T defaultValue, CancellationToken cancellationToken);    
        Task<T> FindArticleByNormalizedTitleAsync(string title, CancellationToken cancellationToken);
        Task<T> FindArticleByNormalizedTitleOrDefaultAsync(string title, T defaultValue, CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(int howManyElements, CancellationToken cancellationToken);      
        Task<DeleteResult> DeleteArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken);      
        Task<DeleteResult> DeleteArticleByNormalizedTitleAsync(string title, CancellationToken cancellationToken);
        Task<UpdateResult> UpdateArticleViewsCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken);
        Task<UpdateResult> UpdateArticleLikesCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken);
        Task<UpdateResult> UpdateArticleDislikesCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplaceArticleAsync(T entity, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplaceArticleByNormalizedTitleAsync(string title, T entity, CancellationToken cancellationToken);
    }
}