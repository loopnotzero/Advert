using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.IStores;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class ArticlesViewCountManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesViewCountStore<T> Store { get; set; }

        public ArticlesViewCountManager(IArticlesViewCountStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task<T> FindArticlesViewCountByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.FindArticlesViewCountByArticleIdAsync(id, cancellationToken);
        }

        public async Task<long> CountArticlesViewCountByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.CountArticlesViewCountByArticleIdAsync(id, cancellationToken);
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