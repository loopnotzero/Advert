using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public interface IPostCommentsCollection<T> : IDisposable where T : IPostComment
    {        
        Task CreatePostCommentAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindPostCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<long> EstimatedPostCommentsCountAsync(CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsAsync(int offset, int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsAsync(int offset, int? howManyElements, SortDefinition sortDef, CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<UpdateResult> DeletePostCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<ReplaceOneResult> UpdatePostCommentByIdAsync(ObjectId commentId, T entity, CancellationToken cancellationToken);
    }
}