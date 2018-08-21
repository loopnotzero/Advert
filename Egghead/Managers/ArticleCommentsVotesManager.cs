using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Common.Stores;
using MongoDB.Bson;

namespace Egghead.Managers
{
    public class ArticleCommentsVotesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticleCommentsVotesStore<T> Store { get; set; }

        public ArticleCommentsVotesManager(IArticleCommentsVotesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task<long> CountArticleCommentVotesAsync(ObjectId articleId, ObjectId commentId, VoteType voteType)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
            
            if (commentId == null)
            {
                throw new ArgumentNullException(nameof(commentId));
            }
            
            if (voteType == VoteType.None)
            {
                throw new ArgumentNullException(nameof(voteType));
            }

            return await Store.CountArticleCommentVotesAsync(articleId, commentId, voteType, CancellationToken);
        }

        public async Task<T> FindArticleCommentVoteAsync(ObjectId articleId, ObjectId commentId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
            
            if (commentId == null)
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            return await Store.FindArticleCommentVoteAsync(articleId, commentId, CancellationToken);
        }
        
        public async Task<OperationResult> CreateArticleCommentVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.CreateArticleCommentVoteAsync(entity, CancellationToken);
        }
        
        public async Task<OperationResult> UpdateArticleCommentVoteAsync(ObjectId voteId, VoteType voteType)
        {
            ThrowIfDisposed();
            
            if (voteId == null)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.UpdateArticleCommentVoteAsync(voteId, voteType, CancellationToken);
        }
        
        public async Task<OperationResult> DeleteArticleCommentVoteAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId == null)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.DeleteArticleCommentVoteAsync(voteId, CancellationToken);
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