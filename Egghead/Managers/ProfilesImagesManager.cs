using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Egghead.Managers
{
    public class ProfilesImagesManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        /// <summary>
        /// The <see cref="T:Microsoft.AspNetCore.Identity.ILookupNormalizer" /> used to normalize things like user and role names.
        /// </summary>
        private ILookupNormalizer KeyNormalizer { get; set; }

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IProfilesImagesStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;
        
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
        
        public ProfilesImagesManager(IProfilesImagesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            KeyNormalizer = keyNormalizer;
        }
        
        public async Task<T> GetProfileImageByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();          
            return await Store.GetProfileImageByProfileIdAsync(profileId, CancellationToken);
        }

        public async Task<OperationResult> CreateProfileImageAsync(T entity)
        {
            ThrowIfDisposed();
            return await Store.CreateProfileImageAsync(entity, CancellationToken);
        }      
    }
}