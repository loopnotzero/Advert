using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common;
using Advert.Common.AdsTopics;
using Advert.MongoDbStorage.AdsTopics;
using Advert.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.Managers
{
    public class AdsTopicsManager<T> : IDisposable where T : class
    {
        private bool _disposed;       
        private readonly ILogger _logger;      
        private ILookupNormalizer _keyNormalizer { get; set; }
        protected internal IAdsTopicsStore<T> _store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public AdsTopicsManager(IAdsTopicsStore<T> store, ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = loggerFactory.CreateLogger<AdsTopicsManager<T>>();
            _keyNormalizer = keyNormalizer;
        }

        public async Task CreateAdsTopicAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await _store.CreateAdsTopicAsync(entity, CancellationToken);
        }
        
        public async Task UpdateAdsTopicAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await _store.UpdateAdsTopicAsync(entity, CancellationToken);
        }

        public async Task<T> FindAdsTopicByIdAsync(ObjectId adsId)
        {
            ThrowIfDisposed();

            if (adsId == null)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.FindAdsTopicByIdAsync(adsId, CancellationToken);
        }
            
        public async Task<long> CountAdsTopicsByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();
            return await _store.CountAdsTopicsByProfileIdAsync(profileId, CancellationToken);
        }

        public async Task<long> EstimatedAdsTopicsCountAsync()
        {
            ThrowIfDisposed();
            return await _store.EstimatedAdsTopicsCountAsync(CancellationToken);           
        } 

        public async Task<List<T>> FindAdsTopicsAsync(int? howManyElements)
        {
            ThrowIfDisposed();          
            return await _store.FindAdsTopicsAsync(howManyElements, CancellationToken);
        }
        
        public async Task<List<T>> FindAdsTopicsAsync(int offset, int? howManyElements)
        {
            ThrowIfDisposed();          
            return await _store.FindAdsTopicsAsync(offset, howManyElements, CancellationToken);
        }
      
        public async Task<List<T>> FindAdsTopicsByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();
            
            if (profileId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(profileId));
            }

            return await _store.FindAdsTopicsByProfileIdAsync(profileId, CancellationToken);
        }

        public async Task<List<T>> FindAdsTopicsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(keyword))
            {
                throw new ArgumentNullException(nameof(keyword));
            }

            return await _store.FindAdsTopicsWhichContainsKeywordAsync(offset, howManyElements, keyword, CancellationToken);
        }

        public async Task<UpdateResult> UpdateAdsTopicViewsCountByAdsId(ObjectId adsId, long viewsCount)
        {
            ThrowIfDisposed();

            if (adsId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.UpdateAdsTopicViewsCountByAdsIdAsync(adsId, viewsCount, CancellationToken);
        }
        
        public async Task<UpdateResult> UpdateAdsTopicLikesCountByAdsId(ObjectId adsId, long votesCount)
        {
            ThrowIfDisposed();

            if (adsId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.UpdateAdsTopicLikesCountByAdsIdAsync(adsId, votesCount, CancellationToken);
        }

        public async Task<DeleteResult> DeleteAdsTopicByIdAsync(ObjectId adsId)
        {
            ThrowIfDisposed();

            if (adsId == null)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await _store.DeleteAdsTopicByIdAsync(adsId, CancellationToken);
        }

        public async Task<ReplaceOneResult> ReplaceAdsTopicAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _store.ReplaceAdsTopicAsync(entity, CancellationToken);
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