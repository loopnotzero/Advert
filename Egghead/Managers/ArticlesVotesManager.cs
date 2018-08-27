using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Common.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Egghead.Managers
{
    public class ArticlesLikesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        /// <summary>
        /// The <see cref="T:Microsoft.AspNetCore.Identity.ILookupNormalizer" /> used to normalize things like user and role names.
        /// </summary>
        private ILookupNormalizer KeyNormalizer { get; set; }
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesVotesStore<T> Store { get; set; }

        public ArticlesLikesManager(IArticlesVotesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            KeyNormalizer = keyNormalizer;
        }

        public async Task<T> FindArticleVoteAsync(ObjectId articleId, VoteType voteType, string byWhoNormalized)
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

            byWhoNormalized = NormalizeKey(byWhoNormalized);

            return await Store.FindArticleVoteAsync(articleId, voteType, byWhoNormalized, CancellationToken);
        }

        public async Task<long> CountArticleVotesAsync(ObjectId articleId, VoteType voteType)
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
        
        private string NormalizeKey(string key)
        {
            return KeyNormalizer != null ? KeyNormalizer.Normalize(key) : key;
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