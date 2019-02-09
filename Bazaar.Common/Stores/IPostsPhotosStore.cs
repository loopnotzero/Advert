using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using MongoDB.Bson;

namespace Bazaar.Common.Stores
{
    public interface IPostsPhotosStore<T> : IDisposable where T : IPostPhoto
    {
        Task CreatePostPhotoAsync(T entity, CancellationToken cancellationToken);
        Task DeletePostPhotoById(ObjectId photoId, CancellationToken cancellationToken);
        Task DeletePostPhotosByPostId(ObjectId postId, CancellationToken cancellationToken);
        Task DeletePostPhotoByFileName(string fileName, CancellationToken cancellationToken);
        Task<List<T>> GetPostPhotosByPostIdAsync(ObjectId postId, CancellationToken cancellationToken);
    }
}