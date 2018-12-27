using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Stores;
using MongoDB.Bson;

namespace Egghead.Managers
{
    public class AdvertisementsViewCountManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IAdvertisementsViewCountStore<T> Store { get; set; }

        public AdvertisementsViewCountManager(IAdvertisementsViewCountStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task CreateAdvertisementViewsCountAsync(T entity)
        {
            ThrowIfDisposed();
            await Store.CreateAdvertisementViewsCountAsync(entity, CancellationToken);
        }

        public async Task<long> CountAdvertisementViewsCountAsync(ObjectId adsId)
        {
            ThrowIfDisposed();

            if (adsId == null)
            {
                throw new ArgumentNullException(nameof(adsId));
            }

            return await Store.CountAdvertisementViewsCountByAdsIdAsync(adsId, CancellationToken);
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