using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Advertisements;
using Egghead.Models.Advertisements;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Stores;
using MongoDB.Bson;

namespace Egghead.Managers
{
    public class AdvertisementsCommentsManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IAdvertisementsCommentsStore<T> Store { get; set; }
        
        public AdvertisementsCommentsManager(IAdvertisementsCommentsStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task CreateAdvertisementComment(string collectionName, T entity)
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

            var advertisementComments = Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken);

            await advertisementComments.CreateAdvertisementCommentAsync(entity, CancellationToken);
        }
        
        public async Task UpdateAdvertisementCommentByIdAsync(string collectionName, ObjectId commentId, T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var advertisementComments = Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken);

            await advertisementComments.UpdateAdvertisementCommentByIdAsync(commentId, entity, CancellationToken);
        }

        public async Task<T> FindAdvertisementCommentById(string collectionName, ObjectId commendId)
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

            return await Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken).FindAdvertisementCommentByIdAsync(commendId, CancellationToken);
        }
        
        public async Task<long> EstimatedAdvertisementCommentsByAdsIdAsync(string collectionName)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken).EstimatedAdvertisementCommentsCountAsync(CancellationToken);
        }
        
        public async Task<List<T>> FindAdvertisementCommentsAsync(string collectionName, int? howManyElements)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken).FindAdvertisementCommentsAsync(howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindAdvertisementCommentsAsync(string collectionName, int offset, int? howManyElements)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken).FindAdvertisementCommentsAsync(offset, howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindAdvertisementCommentsAsync(string collectionName, int offset, int? howManyElements, SortDefinition sortDef)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken).FindAdvertisementCommentsAsync(offset, howManyElements, sortDef, CancellationToken);
        }
        
        public async Task<List<T>> FindAdvertisementCommentsByProfileIdAsync(string collectionName, ObjectId profileId)
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

            return await Store.GetAdvertisementCommentsCollection(collectionName, CancellationToken).FindAdvertisementCommentsByProfileIdAsync(profileId, CancellationToken);
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