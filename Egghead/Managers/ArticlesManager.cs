using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class ArticlesManager<T> : IDisposable where T : class
    {
        private bool _disposed;       
        private readonly ILogger _logger;      
        private ILookupNormalizer _keyNormalizer { get; set; }
        protected internal IArticlesStore<T> _store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ArticlesManager(IArticlesStore<T> store, ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = loggerFactory.CreateLogger<ArticlesManager<T>>();
            _keyNormalizer = keyNormalizer;
        }

        public async Task CreateArticleAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await _store.CreateArticleAsync(entity, CancellationToken);
        }

        public async Task<T> FindArticleByIdAsync(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await _store.FindArticleByIdAsync(articleId, CancellationToken);
        }
        
        public async Task<long> CountArticlesByNormalizedEmail(string email)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await _store.CountArticlesByNormalizedEmail(NormalizeKey(email), CancellationToken);
        }

        public async Task<List<T>> FindArticlesAsync(int howManyElements)
        {
            ThrowIfDisposed();          
            return await _store.FindArticlesAsync(howManyElements, CancellationToken);
        }
       
        public async Task<UpdateResult> UpdateArticleViewsCountByArticleId(ObjectId articleId, long viewsCount)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await _store.UpdateArticleViewsCountByArticleIdAsync(articleId, viewsCount, CancellationToken);
        }
        
        public async Task<UpdateResult> UpdateArticleLikesCountByArticleId(ObjectId articleId, long votesCount)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await _store.UpdateArticleLikesCountByArticleIdAsync(articleId, votesCount, CancellationToken);
        }

        public async Task<UpdateResult> UpdateArticleDislikesCountByArticleId(ObjectId articleId, long votesCount)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await _store.UpdateArticleDislikesCountByArticleIdAsync(articleId, votesCount, CancellationToken);
        }

        public async Task<DeleteResult> DeleteArticleByIdAsync(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await _store.DeleteArticleByIdAsync(articleId, CancellationToken);
        }

        public async Task<ReplaceOneResult> ReplaceArticleAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _store.ReplaceArticleAsync(entity, CancellationToken);
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