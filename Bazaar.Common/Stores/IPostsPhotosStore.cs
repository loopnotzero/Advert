using System;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;

namespace Bazaar.Common.Stores
{
    public interface IPostsPhotosStore<T> : IDisposable where T : IPostPhotos
    {
        Task CreatePostPhotosAsync(T entity, CancellationToken cancellationToken);
    }
}