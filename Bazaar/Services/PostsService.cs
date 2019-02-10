using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Bazaar.Normalizers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Services
{
    public class PostsService<T> : IDisposable where T : IPost
    {
        private bool _disposed;       
        private readonly ILogger _logger;      
        private ILookupNormalizer _keyNormalizer { get; set; }
        protected internal IPostsStore<T> _store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public PostsService(IPostsStore<T> store, ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = loggerFactory.CreateLogger<PostsService<T>>();
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
            
        public async Task<long> CountPostsByIdentityNameAsync(string identityName)
        {
            ThrowIfDisposed();
            return await _store.CountPostsByIdentityNameAsync(identityName, CancellationToken);
        }

        public async Task<long> EstimatedPostsCountAsync()
        {
            ThrowIfDisposed();
            return await _store.EstimatedPostsCountAsync(CancellationToken);           
        } 

        public async Task<List<T>> FindPostsAsync(int offset, int? limit)
        {
            ThrowIfDisposed();          
            return await _store.FindPostsAsync(offset, limit, CancellationToken);
        }
        
        public async Task<List<T>> FindPostsAsync(List<ObjectId> list)
        {
            ThrowIfDisposed();          
            return await _store.FindPostsAsync(list, CancellationToken);
        }
                   
        public async Task<List<T>> FindPostsByIdentityNameAsync(string identityName, int offset, int? limit)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentNullException(nameof(identityName));
            }

            return await _store.FindPostsByIdentityNameAsync(_keyNormalizer.NormalizeKey(identityName), offset, limit, CancellationToken);
        }

        public async Task<List<T>> FindHiddenPostsByIdentityNameAsync(string identityName, int offset, int? limit)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentNullException(nameof(identityName));
            }
            
            return await _store.FindHiddenPostsByIdentityNameAsync(_keyNormalizer.NormalizeKey(identityName), offset, limit, CancellationToken);
        }

        public async Task<List<T>> FindPostsByKeywordAsync(int offset, int? limit, string keyword)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(keyword))
            {
                throw new ArgumentNullException(nameof(keyword));
            }

            return await _store.FindPostsByKeywordAsync(offset, limit, keyword, CancellationToken);
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