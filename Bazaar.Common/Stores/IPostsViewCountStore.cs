using System;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using MongoDB.Bson;

namespace Bazaar.Common.Stores
{
    //todo: Move this interface to Common
    public interface IPostsViewCountStore<T> : IDisposable where T : IPostViewsCount
    {
        Task CreatePostViewsCountAsync(T entity , CancellationToken cancellationToken);
        Task<T> FindPostViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken);
        Task<long> CountPostViewsCountByPostIdAsync(ObjectId postId, CancellationToken cancellationToken);
    }
}