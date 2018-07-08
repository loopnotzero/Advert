using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;

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

        public async Task SetNormalizedTitleAsync(T entity, string normalizedTitle)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.SetNormalizedTitleAsync(entity, normalizedTitle, CancellationToken);
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

        public async Task<T> FindArticleByTitleAsync(string articleTitle)
        {
            ThrowIfDisposed();

            if (articleTitle == null)
            {
                throw new ArgumentNullException(nameof(articleTitle));
            }

            var article = await Store.FindArticleByTitleAsync(articleTitle.ToUpper(), CancellationToken);

            return article;
        }
        
        public async Task<List<T>> GetArticles()
        {
            ThrowIfDisposed();          
            return await Store.GetArticles(CancellationToken);
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

        public async Task<OperationResult> UpdateArticleByIdAsync(string articleId, T entity)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.UpdateArticleByIdAsync(articleId, entity, CancellationToken);
        }

        public async Task<OperationResult> UpdateArticleByTitleAsync(string articleTitle, T entity)
        {
            ThrowIfDisposed();

            if (articleTitle == null)
            {
                throw new ArgumentNullException(nameof(articleTitle));
            }

            return await Store.UpdateArticleByTitleAsync(articleTitle.ToUpper(), entity, CancellationToken);
        }

        public async Task<OperationResult> DeleteArticleByIdAsync(string articleId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.DeleteArticleByIdAsync(articleId, CancellationToken);
        }

        public async Task<OperationResult> DeleteArticleByTitleAsync(string articleTitle)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(articleTitle))
            {
                throw new ArgumentNullException(nameof(articleTitle));
            }

            return await Store.DeleteArticleByTitleAsync(articleTitle.ToUpper(), CancellationToken);
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