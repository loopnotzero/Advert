using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Advertisements;
using Egghead.MongoDbStorage.Stores;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class AdvertisementCommentsVotesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IAdvertisementCommentsVotesStore<T> Store { get; set; }

        public AdvertisementCommentsVotesManager(IAdvertisementCommentsVotesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task CreateAdvertisementCommentVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreateAdvertisementCommentVoteAsync(entity, CancellationToken);
        }
       
        public async Task<T> FindAdvertisementCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue)
        {
            ThrowIfDisposed();           
            return await Store.FindAdvertisementCommentVoteOrDefaultAsync(commentId, profileId, defaultValue, CancellationToken);
        }
        
        public async Task<long> CountAdvertisementCommentVotesByCommentIdAsync(ObjectId commentId)
        {
            ThrowIfDisposed();           
            return await Store.CountAdvertisementCommentVotesByCommentIdAsync(commentId, CancellationToken);
        }
        
        public async Task<DeleteResult> DeleteAdvertisementCommentVoteByIdAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId == null)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.DeleteAdvertisementCommentVoteByIdAsync(voteId, CancellationToken);
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