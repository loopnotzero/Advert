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
        Task UpdateArticleAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken);
        Task<T> FindArticleByIdOrDefaultAsync(ObjectId articleId, T defaultValue, CancellationToken cancellationToken);    
        Task<long> EstimatedArticlesCountAsync(CancellationToken cancellationToken);
        Task<long> CountArticlesByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindArticlesAsync(int offset, int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindArticlesByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken);      
        Task<UpdateResult> UpdateArticleViewsCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken);
        Task<UpdateResult> UpdateArticleLikesCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplaceArticleAsync(T entity, CancellationToken cancellationToken);
    }
}