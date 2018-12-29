using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.Stores;
using MongoDB.Bson;
using Remotion.Linq.Clauses;

namespace Egghead.Managers
{
    public class AdsTopicCommentsVotesAggregationManager<T, A> : IDisposable where T : class where A : class 
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IAdsTopicCommentsVotesAggregationStore<T, A> Store { get; set; }

        public AdsTopicCommentsVotesAggregationManager(IAdsTopicCommentsVotesAggregationStore<T, A> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<A> CountAdsTopicCommentVotesByCommentIdAsync(ObjectId commentId)
        {
            ThrowIfDisposed();           
            return await Store.CountAdsTopicCommentVotesByCommentIdAsync(commentId, CancellationToken);
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