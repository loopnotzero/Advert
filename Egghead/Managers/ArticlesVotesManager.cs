using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
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
        protected internal IArticlesVotesStore<T> Store { get; set; }

        public ArticlesLikesManager(IArticlesVotesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<T> FindArticleVoteAsync(string articleId, VoteType voteType, string byWhoNormalized)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            if (voteType == VoteType.None)
            {
                throw new ArgumentNullException(nameof(voteType));
            }

            if (byWhoNormalized == null)
            {
                throw new ArgumentNullException(nameof(byWhoNormalized));
            }

            return await Store.FindArticleVoteAsync(articleId, voteType, byWhoNormalized, CancellationToken);
        }

        public async Task<long> CountArticleVotesAsync(string articleId, VoteType voteType)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            if (voteType == VoteType.None)
            {
                throw new ArgumentNullException(nameof(voteType));
            }

            return await Store.CountArticleVotesAsync(articleId, voteType, CancellationToken);
        }

        public async Task<OperationResult> CreateArticleVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.CreateArticleVoteAsync(entity, CancellationToken);
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