using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;

namespace Egghead.Managers
{
    public class ArticleCommentsLikesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticleCommentsLikesStore<T> Store { get; set; }

        public ArticleCommentsLikesManager(IArticleCommentsLikesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<T> FindArticleCommentLikesByArticleCommentIdAsync(string articleId, String commentId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticleCommentLikesByArticleCommentIdAsync(articleId, commentId, CancellationToken);
        }

        public async Task<T> FindArticleCommentDislikesByArticleCommentIdAsync(string articleId, string commentId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.FindArticleCommentDislikesByArticleCommentIdAsync(articleId, commentId, CancellationToken);
        }

        public async Task<long> CountArticleCommentLikesByArticleCommentIdAsync(string articleId, string commentId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticleCommentLikesByArticleCommentIdAsync(articleId, commentId, CancellationToken);
        }

        public async Task<long> CountArticleCommentDislikesByArticleCommentIdAsync(string articleId, string commentId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticleCommentDislikesByArticleCommentIdAsync(articleId, commentId, CancellationToken);
        }

        public async Task<OperationResult> SetArticleCommentLikeAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.SetArticleCommentLikeAsync(entity, CancellationToken);
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