using System;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common;
using Advert.MongoDbStorage.Stores;
using MongoDB.Bson;

namespace Advert.Managers
{
    public class AdsTopicsViewsCountManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IAdsTopicsViewCountStore<T> Store { get; set; }

        public AdsTopicsViewsCountManager(IAdsTopicsViewCountStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task CreateAdsTopicViewsCountAsync(T entity)
        {
            ThrowIfDisposed();
            await Store.CreateAdsTopicViewsCountAsync(entity, CancellationToken);
        }

        public async Task<long> CountAdsTopicViewsCountAsync(ObjectId adsId)
        {
            ThrowIfDisposed();

            if (adsId == null)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await Store.CountAdsTopicViewsCountByAdsIdAsync(adsId, CancellationToken);
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