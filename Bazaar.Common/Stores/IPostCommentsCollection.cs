using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Common.Stores
{
    public interface IPostCommentsCollection<T> : IDisposable where T : IPostComment
    {        
        Task CreatePostCommentAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindPostCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<long> CountPostCommentsCountAsync(CancellationToken cancellationToken);
        Task<long> EstimatedPostCommentsCountAsync(CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsAsync(int? limit, CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsAsync(int offset, int? limit, CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsAsync(int offset, int? limit, SortDefinition sortDef, CancellationToken cancellationToken);
        Task<List<T>> FindPostCommentsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<UpdateResult> DeletePostCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplacePostCommentAsync(ObjectId commentId, T entity, CancellationToken cancellationToken);
    }
}