using System;
using Bazaar.Common.Posts;

namespace Bazaar.Common.Stores
{
    public interface IPostsPhotosStore<T> : IDisposable where T : IPostPhotos
    {
        
    }
}