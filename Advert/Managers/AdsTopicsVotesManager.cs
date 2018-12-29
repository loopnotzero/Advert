using System;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common;
using Advert.Common.AdsTopics;
using Advert.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.Managers
{
    public class AdsTopicsVotesManager<T> : IDisposable where T : class
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
        protected internal IAdsTopicsVotesStore<T> Store { get; set; }

        public AdsTopicsVotesManager(IAdsTopicsVotesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            KeyNormalizer = keyNormalizer;
        }

        public async Task CreateAdsTopicVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreateAdsTopicVoteAsync(entity, CancellationToken);
        }

        public async Task<T> FindAdsTopicVoteByNormalizedEmailAsync(ObjectId adsId, string email)
        {
            ThrowIfDisposed();

            if (adsId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await Store.FindAdsTopicVoteByNormalizedEmailAsync(adsId, NormalizeKey(email), CancellationToken);
        }

        public async Task<long> CountAdsTopicVotesByVoteTypeAsync(ObjectId adsId, VoteType voteType)
        {
            ThrowIfDisposed();

            if (adsId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await Store.CountAdsTopicVotesByVoteTypeAsync(adsId, voteType, CancellationToken);
        }

        public async Task<DeleteResult> DeleteAdsTopicVoteByIdAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.DeleteAdsTopicVoteByIdAsync(voteId, CancellationToken);
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