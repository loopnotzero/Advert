using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IArticleCommentsCollection<T> : IDisposable where T : class
    {        
        Task CreateArticleCommentAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindArticleCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<long> EstimatedArticleCommentsCountAsync(CancellationToken cancellationToken);
        Task<List<T>> FindArticleCommentsAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteArticleCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<ReplaceOneResult> UpdateArticleCommentByIdAsync(ObjectId commentId, T entity, CancellationToken cancellationToken);
    }
}