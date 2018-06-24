using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        protected internal IArticleStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ArticlesManager(IArticleStore<T> store)
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

        public async Task<T> FindArticleByIdAsync(string articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticleByIdAsync(articleId, CancellationToken);
        }

        public async Task<T> FindArticleByTitleAsync(string normalizedTitle)
        {
            ThrowIfDisposed();

            if (normalizedTitle == null)
            {
                throw new ArgumentNullException(nameof(normalizedTitle));
            }

            return await Store.FindArticleByTitleAsync(normalizedTitle, CancellationToken);
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

        public async Task<IdentityResult> DeleteArticleAsync(T article)
        {
            ThrowIfDisposed();

            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            return await Store.DeleteArticleAsync(article, CancellationToken);
        }

        public async Task<IdentityResult> UpdateArticleAsync(T article)
        {
            ThrowIfDisposed();

            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            return await Store.UpdateArticleAsync(article, CancellationToken);
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