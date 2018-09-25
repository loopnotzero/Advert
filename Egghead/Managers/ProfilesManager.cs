using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Egghead.Managers
{
    public class ProfilesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        /// <summary>
        /// The <see cref="T:Microsoft.AspNetCore.Identity.ILookupNormalizer" /> used to normalize things like user and role names.
        /// </summary>
        private readonly ILookupNormalizer _keyNormalizer;
      
        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IProfilesStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;
        
        public ProfilesManager(IProfilesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            _keyNormalizer = keyNormalizer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
 
        public async Task CreateProfileAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreateProfileAsync(entity, CancellationToken);
        }

        public async Task<T> FindProfileByIdAsync(ObjectId id)
        {
            ThrowIfDisposed();

            if (id == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(id)); 
            }
            
            return await Store.FindProfileByIdAsync(id, CancellationToken);
        }

        public async Task<T> FindProfileByNormalizedEmail(string email)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email)); 
            }
            
            return await Store.FindProfileByNormalizedEmailAsync(email, CancellationToken);
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