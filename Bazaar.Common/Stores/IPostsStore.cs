using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Common.Stores
{
    //todo: Move this interface to Common
    public interface IPostsStore<T> : IDisposable where T : IPost
    {      
        Task CreatePostAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindPostByIdAsync(ObjectId postId, CancellationToken cancellationToken);
        Task<T> FindPostByIdOrDefaultAsync(ObjectId postId, T defaultValue, CancellationToken cancellationToken);    
        Task<long> EstimatedPostsCountAsync(CancellationToken cancellationToken);
        Task<long> CountPostsByIdentityNameAsync(string identityName, CancellationToken cancellationToken);
        Task<List<T>> FindPostsAsync(List<ObjectId> list, CancellationToken cancellationToken);
        Task<List<T>> FindPostsAsync(int offset, int? limit, CancellationToken cancellationToken);
        Task<List<T>> FindPostsByKeywordAsync(int offset, int? limit, string keyword, CancellationToken cancellationToken);
        Task<List<T>> FindPostsByIdentityNameAsync(string identityName, int offset, int? limit,
            CancellationToken cancellationToken);
        Task<List<T>> FindHiddenPostsByIdentityNameAsync(string identityName, int offset, int? limit,
            CancellationToken cancellationToken);
        Task<UpdateResult> DeletePostByIdAsync(ObjectId postId, CancellationToken cancellationToken);      
        Task<UpdateResult> UpdatePostViewsCountByPostIdAsync(ObjectId postId, long count, CancellationToken cancellationToken);
        Task<UpdateResult> UpdatePostLikesCountByPostIdAsync(ObjectId postId, long count, CancellationToken cancellationToken);
        Task<ReplaceOneResult> UpdatePostAsync(T entity, CancellationToken cancellationToken);
    }
}