using System;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Profiles;
using Bazaar.Common.Stores;
using Bazaar.Normalizers;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Services
{
    public class ProfilesService<T> : IDisposable where T : IProfile
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
        
        public ProfilesService(IProfilesStore<T> store, ILookupNormalizer keyNormalizer)
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
        
        public async Task<ReplaceOneResult> UpdateProfileAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.UpdateProfileAsync(entity, CancellationToken);
        }

        public async Task<T> FindProfileByIdAsync(ObjectId profileId)
        {
            ThrowIfDisposed();

            if (profileId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(profileId)); 
            }
            
            return await Store.FindProfileByIdAsync(profileId, CancellationToken);
        }

        public async Task<T> FindProfileByIdOrDefaultAsync(ObjectId profileId, T defaultValue)
        {
            ThrowIfDisposed();

            if (profileId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(profileId)); 
            }
            
            return await Store.FindProfileByIdOrDefaultAsync(profileId, CancellationToken);
        }
        
        public async Task<IProfile> FindProfileByNormalizedName(string profileName)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(profileName))
            {
                throw new ArgumentNullException(nameof(profileName)); 
            }
            
            return await Store.FindProfileByNormalizedName(_keyNormalizer.NormalizeKey(profileName), CancellationToken);
        }
        
        public async Task<IProfile> FindProfileByNormalizedNameOrDefaultAsync(string profileName, T defaultValue)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(profileName))
            {
                throw new ArgumentNullException(nameof(profileName)); 
            }
            
            return await Store.FindProfileByNormalizedNameOrDefault(_keyNormalizer.NormalizeKey(profileName), defaultValue, CancellationToken);
        }
        
        public async Task<T> FindProfileByIdentityName(string email)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email)); 
            }
            
            return await Store.FindProfileByIdentityNameAsync(_keyNormalizer.NormalizeKey(email), CancellationToken);
        }
        
        public async Task<T> FindProfileByIdentityNameOrDefaultAsync(string email, T defaultValue)
        {
            ThrowIfDisposed();
            
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email)); 
            }
            
            return await Store.FindProfileByIdentityNameOrDefaultAsync(_keyNormalizer.NormalizeKey(email), defaultValue, CancellationToken);
        }
        
        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            Store.Dispose();

            _disposed = true;
        }

        public void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }   
    }
}