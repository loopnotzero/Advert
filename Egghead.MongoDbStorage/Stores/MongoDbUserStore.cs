﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.Mappings;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbUserStore<T> : IUserEmailStore<T>, IUserPasswordStore<T> where T : MongoDbUser
    {
        private readonly IMongoCollection<T> _collection;

        private MongoDbUserStore()
        {
            EntityMappings.EnsureMongoDbUserConfigured();
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
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);

            return entity?.Id;
        }

        public async Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);

            return entity?.UserName;
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            user.UserName = userName;
            
            return Task.FromResult<object>(null);
        }

        public async Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);

            return entity?.NormalizedUserName;
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {     
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            user.NormalizedUserName = normalizedName ?? user.Email.ToUpper();
            
            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(user, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), user, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken);
            
            return IdentityResult.Success;
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
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }            
            
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
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                  
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);

            return entity?.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
               
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

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
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
                      
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedEmail, email), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<string> GetNormalizedEmailAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
                
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);

            return entity?.Email;
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
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
          
            cancellationToken.ThrowIfCancellationRequested();
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);

            return entity?.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
          
            cancellationToken.ThrowIfCancellationRequested();
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, user.Id), cancellationToken: cancellationToken);

            var entity = await cursor.FirstOrDefaultAsync(cancellationToken);
            
            return entity?.PasswordHash != null;
        }
    }
}