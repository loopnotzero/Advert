using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.Stores;
using MongoDB.Bson;
using MongoDB.Driver;

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
        
        public async Task CreateArticleCommentVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreateArticleCommentVoteAsync(entity, CancellationToken);
        }

        public async Task<long> CountArticleCommentVotesAsync(ObjectId commentId, VoteType voteType)
        {
            ThrowIfDisposed();
            
            if (commentId == null)
            {
                throw new ArgumentNullException(nameof(commentId));
            }
            
            if (voteType == VoteType.None)
            {
                throw new ArgumentNullException(nameof(voteType));
            }

            return await Store.CountArticleCommentVotesAsync(commentId, voteType, CancellationToken);
        }

        public async Task<T> FindArticleCommentVoteAsync(ObjectId commentId)
        {
            ThrowIfDisposed();       
            
            if (commentId == null)
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            return await Store.FindArticleCommentVoteAsync(commentId, CancellationToken);
        }
               
        public async Task<UpdateResult> UpdateArticleCommentVoteAsync(ObjectId voteId, VoteType voteType)
        {
            ThrowIfDisposed();
            
            if (voteId == null)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.UpdateArticleCommentVoteAsync(voteId, voteType, CancellationToken);
        }
        
        public async Task<DeleteResult> DeleteArticleCommentVoteAsync(ObjectId voteId)
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