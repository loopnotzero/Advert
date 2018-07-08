using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;

namespace Egghead.Managers
{
    public class ArticlesLikesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesLikesStore<T> Store { get; set; }

        public ArticlesLikesManager(IArticlesLikesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<T> FindArticlesLikesByArticleIdAsync(string articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticlesLikesByArticleIdAsync(articleId, CancellationToken);
        }

        public async Task<T> FindArticlesDislikesByArticleIdAsync(string articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticlesDislikesByArticleIdAsync(articleId, CancellationToken);
        }

        public async Task<long> CountArticlesLikesByArticleIdAsync(string articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticlesLikesByArticleIdAsync(articleId, CancellationToken);
        }

        public async Task<long> CountArticlesDislikesByArticleIdAsync(string articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticlesDislikesByArticleIdAsync(articleId, CancellationToken);
        }

        public async Task<OperationResult> AddArticleLikeAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.AddArticleLikeAsync(entity, CancellationToken);
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