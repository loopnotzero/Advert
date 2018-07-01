using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.IStores;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class ArticlesViewCountManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesViewCountStore<T> Store { get; set; }
        
        public ArticlesViewCountManager(IArticlesViewCountStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task<T> FindArticlesViewCountByArticleIdAsync(string id)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.FindArticlesViewCountByArticleIdAsync(id, CancellationToken);
        }

        public async Task<long> CountArticlesViewCountByArticleIdAsync(string id)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.CountArticlesViewCountByArticleIdAsync(id, CancellationToken);
        }
        
        public async Task<OperationResult> AddArticlesViewAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.AddArticleViewAsync(entity, CancellationToken);
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