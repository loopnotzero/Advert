using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.AdsTopic;
using Advert.Common.AdsTopics;
using Advert.MongoDbStorage.Stores;
using MongoDB.Bson;

namespace Advert.Managers
{
    public class AdsTopicCommentsManager<T> : IDisposable where T : IAdsTopicComment
    {
        private bool _disposed;
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IAdsTopicsCommentsStore<T> Store { get; set; }
        
        public AdsTopicCommentsManager(IAdsTopicsCommentsStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task CreateAdsTopicComment(string collectionName, T entity)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var adsTopicComments = Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken);

            await adsTopicComments.CreateAdsTopicCommentAsync(entity, CancellationToken);
        }
        
        public async Task UpdateAdsTopicCommentByIdAsync(string collectionName, ObjectId commentId, T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var adsTopicCommentsCollection = Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken);

            await adsTopicCommentsCollection.UpdateAdsTopicCommentByIdAsync(commentId, entity, CancellationToken);
        }

        public async Task<T> FindAdsTopicCommentById(string collectionName, ObjectId commendId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }
            
            if (commendId.Equals(ObjectId.Empty))
            {
                throw new ArgumentNullException(nameof(commendId));
            }

            return await Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken).FindAdsTopicCommentByIdAsync(commendId, CancellationToken);
        }
        
        public async Task<long> EstimatedAdsTopicCommentsByAdsIdAsync(string collectionName)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken).EstimatedAdsTopicCommentsCountAsync(CancellationToken);
        }
        
        public async Task<List<T>> FindAdsTopicCommentsAsync(string collectionName, int? howManyElements)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken).FindAdsTopicCommentsAsync(howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindAdsTopicCommentsAsync(string collectionName, int offset, int? howManyElements)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken).FindAdsTopicCommentsAsync(offset, howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindAdsTopicCommentsAsync(string collectionName, int offset, int? howManyElements, SortDefinition sortDef)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken).FindAdsTopicCommentsAsync(offset, howManyElements, sortDef, CancellationToken);
        }
        
        public async Task<List<T>> FindAdsTopicCommentsByProfileIdAsync(string collectionName, ObjectId profileId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            if (profileId.Equals(ObjectId.Empty))
            {
                throw new ArgumentNullException(nameof(profileId)); 
            }

            return await Store.GetAdsTopicCommentsCollection(collectionName, CancellationToken).FindAdsTopicCommentsByProfileIdAsync(profileId, CancellationToken);
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