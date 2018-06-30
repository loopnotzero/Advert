using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.IStores;

namespace Egghead.Managers
{
    public class ArticlesLikesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesLikesStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ArticlesLikesManager(IArticlesLikesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<T> FindArticlesLikesByArticleIdAsync(string id)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.FindArticlesLikesByArticleIdAsync(id, CancellationToken);
        }

        public async Task<T> FindArticlesUnlikesByArticleIdAsync(string id)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.FindArticlesUnlikesByArticleIdAsync(id, CancellationToken);
        }

        public async Task<long> CountArticlesLikesByArticleIdAsync(string id)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.CountArticlesLikesByArticleIdAsync(id, CancellationToken);
        }

        public async Task<long> CountArticlesUnlikesByArticleIdAsync(string id)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.CountArticlesUnlikesByArticleIdAsync(id, CancellationToken);
        }

        public async Task<OperationResult> AddLikeAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.AddLikeAsync(entity, CancellationToken);
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