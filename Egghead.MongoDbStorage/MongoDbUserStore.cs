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
    public class MongoDbUserStore<T> : IUserStore<T> where T : MongoDbIdentityUser
    {
        private readonly IMongoCollection<T> _users;
         
        public MongoDbUserStore(IMongoDatabase mongoDatabase) : this()
        {          
            _users = mongoDatabase.GetCollection<T>(MongoDbCollections.Users);
            
            //todo: Create indices
        }
        
        private MongoDbUserStore()
        {
            RegisterWellKnownTypes.EnsureConfigure();
        }

        public void Dispose()
        {
            //throw new System.NotImplementedException();
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
           
            cancellationToken.ThrowIfCancellationRequested();
            
            var query = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.Id, userId));

            return _users.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<T> FindByNameAsync(string email, CancellationToken cancellationToken)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var query = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.Email, email));

            return _users.Find(query).FirstOrDefaultAsync(cancellationToken);
        }
        
        public Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
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

        public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            await _users.InsertOneAsync(user, new InsertOneOptions(), cancellationToken).ConfigureAwait(false);
            
            return IdentityResult.Success;
        }
    }
}