using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.IStores;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class ArticlesViewsManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesViewsStore<T> Store { get; set; }

        public ArticlesViewsManager(IArticlesViewsStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task<T> FindArticlesViewsByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticlesViewsByArticleIdAsync(articleId, cancellationToken);
        }

        public async Task<long> CountArticlesViewsByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticlesViewsByArticleIdAsync(articleId, cancellationToken);
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