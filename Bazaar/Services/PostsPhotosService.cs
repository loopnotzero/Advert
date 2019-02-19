using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Bazaar.Services
{
    public class PostsPhotosService<T> : IDisposable where T : IPostPhoto
    {      
        private bool _disposed;    
        
        private readonly ILogger _logger;      
        private ILookupNormalizer _keyNormalizer { get; set; }
        protected internal IPostsPhotosStore<T> _store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public PostsPhotosService(IPostsPhotosStore<T> store, ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = loggerFactory.CreateLogger<PostsPhotosService<T>>();
            _keyNormalizer = keyNormalizer;
        }
     
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public async Task CreatePostPhotosAsync(T entity)
        {
            ThrowIfDisposed();
            await _store.CreatePostPhotoAsync(entity, CancellationToken);
        }

        public async Task<List<T>> GetPostPhotosByPostIdAsync(ObjectId postId)
        {
            ThrowIfDisposed();
            return await _store.GetPostPhotosByPostIdAsync(postId, CancellationToken);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _store.Dispose();

            _disposed = true;
        }
        
        private string NormalizeKey(string key)
        {
            return _keyNormalizer != null ? _keyNormalizer.Normalize(key) : key;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}