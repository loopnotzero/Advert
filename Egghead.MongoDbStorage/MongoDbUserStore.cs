using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Identity;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage
{
    public class MongoDbUserStore<T> : IUserEmailStore<T>, IUserPasswordStore<T> where T : MongoDbIdentityUser
    {
        private readonly IMongoCollection<T> _collection;
         
        private MongoDbUserStore()
        {
            RegisterWellKnownTypes.EnsureConfigure();
        }
        
        public MongoDbUserStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Users);
            
            //todo: Create indices
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(T user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<T> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));
          
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x.Email, email);
            var result = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
            var entity = await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return entity;
        }

        public Task<string> GetNormalizedEmailAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(T user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(T user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPasswordHashAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
          
            cancellationToken.ThrowIfCancellationRequested();
            
            //todo: Compute hash

            var filter = Builders<T>.Filter.Eq(x => x.Password, user.Password);
            var result = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
            var entity = await result.FirstOrDefaultAsync(cancellationToken);
            
            return entity?.Password;
        }

        public Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}