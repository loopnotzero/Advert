using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common;
using Advert.Common.AdsTopic;
using Advert.Common.AdsTopics;
using Advert.MongoDbStorage.Stores;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.Managers
{
    public class AdsTopicCommentsVotesManager<T> : IDisposable where T : IAdsTopicCommentVote
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IAdsTopicCommentsVotesStore<T> Store { get; set; }

        public AdsTopicCommentsVotesManager(IAdsTopicCommentsVotesStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task CreateAdsTopicCommentVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreateAdsTopicCommentVoteAsync(entity, CancellationToken);
        }
       
        public async Task<T> FindAdsTopicCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue)
        {
            ThrowIfDisposed();           
            return await Store.FindAdsTopicCommentVoteOrDefaultAsync(commentId, profileId, defaultValue, CancellationToken);
        }
        
        public async Task<long> CountAdsTopicCommentVotesByCommentIdAsync(ObjectId commentId)
        {
            ThrowIfDisposed();           
            return await Store.CountAdsTopicCommentVotesByCommentIdAsync(commentId, CancellationToken);
        }
        
        public async Task<DeleteResult> DeleteAdsTopicCommentVoteByIdAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId == null)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.DeleteAdsTopicCommentVoteByIdAsync(voteId, CancellationToken);
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