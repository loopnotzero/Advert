using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common;
using Advert.Common.Posts;
using Advert.MongoDbStorage.Stores;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.Managers
{
    public class PostCommentsVotesManager<T> : IDisposable where T : IPostCommentVote
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IPostCommentsVotesStore<T> Store { get; set; }

        public PostCommentsVotesManager(IPostCommentsVotesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task CreatePostCommentVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreatePostCommentVoteAsync(entity, CancellationToken);
        }
       
        public async Task<T> FindPostCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue)
        {
            ThrowIfDisposed();

            if (commentId.Equals(ObjectId.Empty))
            {
                throw new ArgumentException(nameof(commentId));
            }

            if (profileId.Equals(ObjectId.Empty))
            {
                throw new ArgumentException(nameof(profileId));
            }
            
            return await Store.FindPostCommentVoteOrDefaultAsync(commentId, profileId, defaultValue, CancellationToken);
        }
        
        public async Task<List<T>> FindPostsCommentsVotesAsync(ObjectId profileId)
        {
            ThrowIfDisposed();

            if (profileId.Equals(ObjectId.Empty))
            {
                throw new ArgumentException(nameof(profileId));
            }
            
            return await Store.FindPostsCommentsVotesAsync(profileId, CancellationToken);
        }
        
        public async Task<long> CountPostCommentVotesByCommentIdAsync(ObjectId commentId)
        {
            ThrowIfDisposed();           
            return await Store.CountPostCommentVotesByCommentIdAsync(commentId, CancellationToken);
        }
        
        public async Task<DeleteResult> DeletePostCommentVoteByIdAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId.Equals(ObjectId.Empty))
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.DeletePostCommentVoteByIdAsync(voteId, CancellationToken);
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