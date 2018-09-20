using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

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

        public async Task SetNormalizedTitleAsync(T entity, string normalizedTitle)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.SetNormalizedTitleAsync(entity, normalizedTitle, CancellationToken);
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

        public async Task<T> FindArticleByNormalizedTitleAsync(string title)
        {
            ThrowIfDisposed();

            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            title = NormalizeKey(title);

            var article = await Store.FindArticleByNormalizedTitleAsync(title.ToUpper(), CancellationToken);

            return article;
        }

        public async Task<List<T>> FindArticlesAsync(int howManyElements)
        {
            ThrowIfDisposed();          
            return await Store.FindArticlesAsync(howManyElements, CancellationToken);
        }

        public async Task<OperationResult> CreateArticleAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            return await Store.CreateArticleAsync(entity, CancellationToken);
        }
       
        public async Task<OperationResult> UpdateArticleByIdAsync(ObjectId articleId, T entity)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.UpdateArticleByIdAsync(articleId, entity, CancellationToken);
        }

        public async Task<OperationResult> UpdateArticleByTitleAsync(string title, T entity)
        {
            ThrowIfDisposed();

            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.UpdateArticleByNormalizedTitleAsync(title.ToUpper(), entity, CancellationToken);
        }

        public async Task<OperationResult> UpdateArticleViewsCountById(ObjectId articleId, long viewsCount)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.UpdateArticleViewsCountById(articleId, viewsCount, CancellationToken);
        }
        
        public async Task<OperationResult> DeleteArticleByIdAsync(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.DeleteArticleByIdAsync(articleId, CancellationToken);
        }

        public async Task<OperationResult> DeleteArticleByTitleAsync(string title)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            title = NormalizeKey(title);

            return await Store.DeleteArticleByNormalizedTitleAsync(title, CancellationToken);
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