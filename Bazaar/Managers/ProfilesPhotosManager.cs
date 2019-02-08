using System;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common;
using Bazaar.Common.Posts;
using Bazaar.Common.Profiles;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Bazaar.Managers
{
    public class ProfilesPhotosManager<T> : IDisposable where T : IProfilePhoto
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
        protected internal IProfilesPhotosStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;
               
        public ProfilesPhotosManager(IProfilesPhotosStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            KeyNormalizer = keyNormalizer;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task CreateProfilePhotoAsync(T entity)
        {
            ThrowIfDisposed();
            await Store.CreateProfilePhotoAsync(entity, CancellationToken);
        }      

        public async Task<T> GetProfilePhotoByProfileIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();          
            return await Store.GetProfilePhotoByProfileIdAsync(profileId, CancellationToken);
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