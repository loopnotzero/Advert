using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.Managers
{
    public class PostsManager<T> : IDisposable where T : IPost
    {
        private bool _disposed;       
        private readonly ILogger _logger;      
        private ILookupNormalizer _keyNormalizer { get; set; }
        protected internal IPostsStore<T> _store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public PostsManager(IPostsStore<T> store, ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = loggerFactory.CreateLogger<PostsManager<T>>();
            _keyNormalizer = keyNormalizer;
        }

        public async Task CreatePostAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await _store.CreatePostAsync(entity, CancellationToken);
        }
             
        public async Task<T> FindPostByIdAsync(ObjectId postId)
        {
            ThrowIfDisposed();

            if (postId == null)
            {
                throw new ArgumentNullException(nameof(postId));
            }

            return await _store.FindPostByIdAsync(postId, CancellationToken);
        }
            
        public async Task<long> CountPostsByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();
            return await _store.CountPostsByProfileIdAsync(profileId, CancellationToken);
        }

        public async Task<long> EstimatedPostsCountAsync()
        {
            ThrowIfDisposed();
            return await _store.EstimatedPostsCountAsync(CancellationToken);           
        } 

        public async Task<List<T>> FindPostsAsync(int? howManyElements)
        {
            ThrowIfDisposed();          
            return await _store.FindPostsAsync(howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindPostsAsync(int offset, int? howManyElements)
        {
            ThrowIfDisposed();          
            return await _store.FindPostsAsync(offset, howManyElements, CancellationToken);
        }
      
        public async Task<List<T>> FindPostsByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();
            
            if (profileId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(profileId));
            }

            return await _store.FindPostsByProfileIdAsync(profileId, CancellationToken);
        }

        public async Task<List<T>> FindPostsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(keyword))
            {
                throw new ArgumentNullException(nameof(keyword));
            }

            return await _store.FindPostsWhichContainsKeywordAsync(offset, howManyElements, keyword, CancellationToken);
        }

        public async Task<UpdateResult> UpdatePostViewsCountByPostId(ObjectId postId, long viewsCount)
        {
            ThrowIfDisposed();

            if (postId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(postId));
            }

            return await _store.UpdatePostViewsCountByPostIdAsync(postId, viewsCount, CancellationToken);
        }
        
        public async Task<UpdateResult> UpdatePostLikesCountByPostId(ObjectId postId, long votesCount)
        {
            ThrowIfDisposed();

            if (postId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(postId));
            }

            return await _store.UpdatePostLikesCountByPostIdAsync(postId, votesCount, CancellationToken);
        }

        public async Task<UpdateResult> DeletePostByIdAsync(ObjectId postId)
        {
            ThrowIfDisposed();

            if (postId == null)
            {
                throw new ArgumentNullException(nameof(postId));
            }

            return await _store.DeletePostByIdAsync(postId, CancellationToken);
        }

        public async Task<ReplaceOneResult> UpdatePostAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            return await _store.UpdatePostAsync(entity, CancellationToken);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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