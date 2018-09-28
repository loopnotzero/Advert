using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class ArticlesManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        /// <summary>
        /// The <see cref="T:Microsoft.AspNetCore.Identity.ILookupNormalizer" /> used to normalize things like user and role names.
        /// </summary>
        private ILookupNormalizer KeyNormalizer { get; set; }

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ArticlesManager(IArticlesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            KeyNormalizer = keyNormalizer;
        }

        public async Task CreateArticleAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await Store.CreateArticleAsync(entity, CancellationToken);
        }

        public async Task<T> FindArticleByIdAsync(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticleByIdAsync(articleId, CancellationToken);
        }
        
        public async Task<long> CountArticlesByNormalizedEmail(string email)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await Store.CountArticlesByNormalizedEmail(NormalizeKey(email), CancellationToken);
        }

        public async Task<List<T>> FindArticlesAsync(int howManyElements)
        {
            ThrowIfDisposed();          
            return await Store.FindArticlesAsync(howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindPopularArticlesByAudienceEngagementAsync(int howManyElements)
        {
            ThrowIfDisposed();          
            return await Store.FindPopularArticlesByAudienceEngagementAsync(howManyElements, CancellationToken);
        }
       
        public async Task<UpdateResult> UpdateArticleViewsCountByArticleId(ObjectId articleId, long viewsCount)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.UpdateArticleViewsCountByArticleIdAsync(articleId, viewsCount, CancellationToken);
        }
        
        public async Task<UpdateResult> UpdateArticleLikesCountByArticleId(ObjectId articleId, long votesCount)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.UpdateArticleLikesCountByArticleIdAsync(articleId, votesCount, CancellationToken);
        }

        public async Task<UpdateResult> UpdateArticleDislikesCountByArticleId(ObjectId articleId, long votesCount)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.UpdateArticleDislikesCountByArticleIdAsync(articleId, votesCount, CancellationToken);
        }

        public async Task<DeleteResult> DeleteArticleByIdAsync(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.DeleteArticleByIdAsync(articleId, CancellationToken);
        }

        public async Task<ReplaceOneResult> ReplaceArticleAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.ReplaceArticleAsync(entity, CancellationToken);
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

            Store.Dispose();

            _disposed = true;
        }
        
        private string NormalizeKey(string key)
        {
            return KeyNormalizer != null ? KeyNormalizer.Normalize(key) : key;
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