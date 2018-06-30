using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.IStores;

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

        public async Task<T> FindArticleByIdAsync(string id)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.FindArticleByIdAsync(id, CancellationToken);
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

        public async Task<OperationResult> CreateArticleAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.CreateArticleAsync(entity, CancellationToken);
        }

        public async Task<OperationResult> UpdateArticleByIdAsync(string id, T entity)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.UpdateArticleByIdAsync(id, entity, CancellationToken);
        }

        public async Task<OperationResult> UpdateArticleByTitleAsync(string title, T entity)
        {
            ThrowIfDisposed();

            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            return await Store.UpdateArticleByTitleAsync(title.ToUpper(), entity, CancellationToken);
        }

        public async Task<OperationResult> DeleteArticleByIdAsync(string id)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.DeleteArticleByIdAsync(id, CancellationToken);
        }

        public async Task<OperationResult> DeleteArticleByTitleAsync(string title)
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