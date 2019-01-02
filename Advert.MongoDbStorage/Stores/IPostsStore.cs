using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IPostsStore<T> : IDisposable where T : IPost
    {      
        Task CreatePostAsync(T entity, CancellationToken cancellationToken);
        Task UpdatePostAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindPostByIdAsync(ObjectId postId, CancellationToken cancellationToken);
        Task<T> FindPostByIdOrDefaultAsync(ObjectId postId, T defaultValue, CancellationToken cancellationToken);    
        Task<long> EstimatedPostsCountAsync(CancellationToken cancellationToken);
        Task<long> CountPostsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindPostsAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindPostsAsync(int offset, int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindPostsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindPostsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword, CancellationToken cancellationToken);
        Task<UpdateResult> DeletePostByIdAsync(ObjectId postId, CancellationToken cancellationToken);      
        Task<UpdateResult> UpdatePostViewsCountByPostIdAsync(ObjectId postId, long count, CancellationToken cancellationToken);
        Task<UpdateResult> UpdatePostLikesCountByPostIdAsync(ObjectId postId, long count, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplacePostAsync(T entity, CancellationToken cancellationToken);
    }
}