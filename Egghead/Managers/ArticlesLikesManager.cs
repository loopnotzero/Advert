using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.IStores;
using MongoDB.Driver;

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

        public async Task<T> FindArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticlesLikesByArticleIdAsync(articleId, cancellationToken);
        }

        public async Task<T> FindArticlesUnlikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticlesUnlikesByArticleIdAsync(articleId, cancellationToken);
        }

        public async Task<long> CountArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticlesLikesByArticleIdAsync(articleId, cancellationToken);
        }

        public async Task<long> CountArticlesUnlikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticlesUnlikesByArticleIdAsync(articleId, cancellationToken);
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