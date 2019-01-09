using System.Threading;
using System.Threading.Tasks;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Users;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbUserStore<T> : IUserEmailStore<T>, IUserPasswordStore<T> where T : MongoDbUser
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbUserStore(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Users);
            //todo: Create indices
        }
        
        public async Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            return entity?._id.ToString();
        }

        public async Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            return entity?.UserName;
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.UserName = userName;         
            return Task.FromResult<object>(null);
        }

        public async Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            return entity?.NormalizedUserName;
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {     
            cancellationToken.ThrowIfCancellationRequested();          
            user.NormalizedUserName = normalizedName ?? user.UserName.ToUpper();           
            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedEmail = user.NormalizedEmail ?? user.Email.ToUpper();
            user.NormalizedUserName = user.NormalizedUserName ?? user.UserName.ToUpper();
            
            await _collection.InsertOneAsync(user, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x._id, user._id), user, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken);           
            return IdentityResult.Success;
        }

        public async Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {      
            cancellationToken.ThrowIfCancellationRequested();
            var asyncCursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, userId), cancellationToken: cancellationToken);
            return await asyncCursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedUserName, normalizedUserName), cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public Task SetEmailAsync(T user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;          
            return Task.FromResult<object>(null);
        }

        public async Task<string> GetEmailAsync(T user, CancellationToken cancellationToken)
        {     
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            return entity?.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            return entity?.EmailConfirmed == true;
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult<object>(null);
        }

        public async Task<T> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {         
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedEmail, email), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<string> GetNormalizedEmailAsync(T user, CancellationToken cancellationToken)
        {  
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            return entity?.NormalizedEmail;
        }

        public Task SetNormalizedEmailAsync(T user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail ?? user.Email.ToUpper();
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(T user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public async Task<string> GetPasswordHashAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();         
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            return entity?.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();         
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, user._id), cancellationToken: cancellationToken);
            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);       
            return entity?.PasswordHash != null;
        }

        public void Dispose()
        {
        }
    }
}