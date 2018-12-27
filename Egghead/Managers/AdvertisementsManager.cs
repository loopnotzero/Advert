using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Advertisements;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class AdvertisementsManager<T> : IDisposable where T : class
    {
        private bool _disposed;       
        private readonly ILogger _logger;      
        private ILookupNormalizer _keyNormalizer { get; set; }
        protected internal IAdvertisementsStore<T> _store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public AdvertisementsManager(IAdvertisementsStore<T> store, ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = loggerFactory.CreateLogger<AdvertisementsManager<T>>();
            _keyNormalizer = keyNormalizer;
        }

        public async Task CreateAdvertisementAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await _store.CreateAdvertisementAsync(entity, CancellationToken);
        }
        
        public async Task UpdateAdvertisementAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await _store.UpdateAdvertisementAsync(entity, CancellationToken);
        }

        public async Task<T> FindAdvertisementByIdAsync(ObjectId adsId)
        {
            ThrowIfDisposed();

            if (adsId == null)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.FindAdvertisementByIdAsync(adsId, CancellationToken);
        }
            
        public async Task<long> CountAdvertisementsByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();
            return await _store.CountAdvertisementsByProfileIdAsync(profileId, CancellationToken);
        }

        public async Task<long> EstimatedAdvertisementsCountAsync()
        {
            ThrowIfDisposed();
            return await _store.EstimatedAdvertisementsCountAsync(CancellationToken);           
        } 

        public async Task<List<T>> FindAdvertisementsAsync(int? howManyElements)
        {
            ThrowIfDisposed();          
            return await _store.FindAdvertisementsAsync(howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindAdvertisementsAsync(int offset, int? howManyElements)
        {
            ThrowIfDisposed();          
            return await _store.FindAdvertisementsAsync(offset, howManyElements, CancellationToken);
        }
      
        public async Task<List<T>> FindAdvertisementsByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();
            
            if (profileId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(profileId));
            }

            return await _store.FindAdvertisementsByProfileIdAsync(profileId, CancellationToken);
        }

        public async Task<List<T>> FindAdvertisementsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(keyword))
            {
                throw new ArgumentNullException(nameof(keyword));
            }

            return await _store.FindAdvertisementsWhichContainsKeywordAsync(offset, howManyElements, keyword, CancellationToken);
        }

        public async Task<UpdateResult> UpdateAdvertisementViewsCountByAdsId(ObjectId adsId, long viewsCount)
        {
            ThrowIfDisposed();

            if (adsId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.UpdateAdvertisementViewsCountByAdsIdAsync(adsId, viewsCount, CancellationToken);
        }
        
        public async Task<UpdateResult> UpdateAdvertisementLikesCountByAdsId(ObjectId adsId, long votesCount)
        {
            ThrowIfDisposed();

            if (adsId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.UpdateAdvertisementLikesCountByAdsIdAsync(adsId, votesCount, CancellationToken);
        }

        public async Task<DeleteResult> DeleteAdvertisementByIdAsync(ObjectId adsId)
        {
            ThrowIfDisposed();

            if (adsId == null)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.DeleteAdvertisementByIdAsync(adsId, CancellationToken);
        }

        public async Task<ReplaceOneResult> ReplaceAdvertisementAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _store.ReplaceAdvertisementAsync(entity, CancellationToken);
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

            _store.Dispose();

            _disposed = true;
        }
        
        private string NormalizeKey(string key)
        {
            return _keyNormalizer != null ? _keyNormalizer.Normalize(key) : key;
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