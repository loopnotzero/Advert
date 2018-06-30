using System;
using System.Threading;
using System.Threading.Tasks;
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

        public ArticlesLikesManager(IArticlesLikesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<T> FindArticlesLikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.FindArticlesLikesByArticleIdAsync(id, cancellationToken);
        }

        public async Task<T> FindArticlesUnlikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.FindArticlesUnlikesByArticleIdAsync(id, cancellationToken);
        }

        public async Task<long> CountArticlesLikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.CountArticlesLikesByArticleIdAsync(id, cancellationToken);
        }

        public async Task<long> CountArticlesUnlikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await Store.CountArticlesUnlikesByArticleIdAsync(id, cancellationToken);
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