using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Identities;
using Egghead.MongoDbStorage.Utils;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
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

        public async Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)         
                throw new ArgumentNullException(nameof(user));            
            
            if (user.Email == null)           
                throw new ArgumentNullException(nameof(user.Email));
            
            cancellationToken.ThrowIfCancellationRequested();

            var asyncCursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, user.Email), cancellationToken: cancellationToken);

            var identityUser = await asyncCursor.FirstOrDefaultAsync(cancellationToken);

            return identityUser?.Id;
        }

        public async Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)            
                throw new ArgumentNullException(nameof(user));
            
            
            if (user.Email == null)            
                throw new ArgumentNullException(nameof(user.Email));
            

            cancellationToken.ThrowIfCancellationRequested();

            var asyncCursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, user.Email), cancellationToken: cancellationToken);

            var identityUser = await asyncCursor.FirstOrDefaultAsync(cancellationToken);

            return identityUser?.Email;
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)            
                throw new ArgumentNullException(nameof(user));
                     
            if (user.Email == null)            
                throw new ArgumentNullException(nameof(user.Email));
            
            cancellationToken.ThrowIfCancellationRequested();

            var asyncCursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, user.Email), cancellationToken: cancellationToken);

            var identityUser = await asyncCursor.FirstOrDefaultAsync(cancellationToken);

            return identityUser?.Email;
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)            
                throw new ArgumentNullException(nameof(user));
            
            if (user.Email == null)
                throw new ArgumentNullException(nameof(user.Email));
                     
            if (user.PasswordHash == null)            
                throw new ArgumentNullException(nameof(user.PasswordHash));
                      
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(user, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            
            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            var asyncCursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, userId), cancellationToken: cancellationToken);

            return await asyncCursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (normalizedUserName == null)          
                throw new ArgumentNullException(nameof(normalizedUserName));            
            
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, normalizedUserName), cancellationToken: cancellationToken);

            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public Task SetEmailAsync(T user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetEmailAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)           
                throw new ArgumentNullException(nameof(user));
            
            if (user.Email == null)            
                throw new ArgumentNullException(nameof(user.Email));
                   
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, user.Email), cancellationToken: cancellationToken);

            var identityUser = await result.FirstOrDefaultAsync(cancellationToken);

            return identityUser?.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)           
                throw new ArgumentNullException(nameof(user));
            
            if (user.Email == null)            
                throw new ArgumentNullException(nameof(user.Email));
                   
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, user.Email), cancellationToken: cancellationToken);

            var identityUser = await result.FirstOrDefaultAsync(cancellationToken);

            return identityUser?.EmailConfirmed ?? false;
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

            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, email), cancellationToken: cancellationToken);

            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<string> GetNormalizedEmailAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)           
                throw new ArgumentNullException(nameof(user));
            
            if (user.Email == null)            
                throw new ArgumentNullException(nameof(user.Email));
                   
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Email, user.Email), cancellationToken: cancellationToken);

            var identityUser = await result.FirstOrDefaultAsync(cancellationToken);

            return identityUser?.Email;
        }

        public Task SetNormalizedEmailAsync(T user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(T user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public async Task<string> GetPasswordHashAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
          
            cancellationToken.ThrowIfCancellationRequested();
            
            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var identityUser = await result.FirstOrDefaultAsync(cancellationToken);

            return identityUser?.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
          
            cancellationToken.ThrowIfCancellationRequested();
            
            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var identityUser = await result.FirstOrDefaultAsync(cancellationToken);

            if (identityUser == null)
            {
                throw new NullReferenceException();
            }

            return identityUser.PasswordHash != null;
        }
    }
}