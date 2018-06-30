using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Models.Articles;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
using Microsoft.AspNetCore.Identity;

namespace Egghead.Managers
{
    public class ArticlesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ArticlesManager(IArticlesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task SetNormalizedTitleAsync(T article, string normalizedTitle)
        {
            ThrowIfDisposed();

            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            await Store.SetNormalizedTitleAsync(article, normalizedTitle, CancellationToken);
        }

        public async Task<T> FindArticleByIdAsync(string objectId)
        {
            ThrowIfDisposed();

            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            return await Store.FindArticleByIdAsync(objectId, CancellationToken);
        }

        public async Task<T> FindArticleByTitleAsync(string title)
        {
            ThrowIfDisposed();

            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            var article = await Store.FindArticleByTitleAsync(title.ToUpper(), CancellationToken);

            return article;
        }
        
        public async Task<List<T>> GetArticles()
        {
            ThrowIfDisposed();          
            return await Store.GetArticles(CancellationToken);
        }

        public async Task<IdentityResult> CreateArticleAsync(T article)
        {
            ThrowIfDisposed();

            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            return await Store.CreateArticleAsync(article, CancellationToken);
        }

        public async Task<IdentityResult> UpdateArticleByIdAsync(string objectId, T article)
        {
            ThrowIfDisposed();

            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            return await Store.UpdateArticleByIdAsync(objectId, article, CancellationToken);
        }

        public async Task<IdentityResult> UpdateArticleByTitleAsync(string title, T article)
        {
            ThrowIfDisposed();

            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            return await Store.UpdateArticleByTitleAsync(title.ToUpper(), article, CancellationToken);
        }

        public async Task<IdentityResult> DeleteArticleByIdAsync(string objectId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            return await Store.DeleteArticleByIdAsync(objectId, CancellationToken);
        }

        public async Task<IdentityResult> DeleteArticleByTitleAsync(string title)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            return await Store.DeleteArticleByTitleAsync(title.ToUpper(), CancellationToken);
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

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }     
    }
}