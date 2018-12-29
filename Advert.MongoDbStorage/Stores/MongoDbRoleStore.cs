using System;
using System.Threading;
using System.Threading.Tasks;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Roles;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbRoleStore<T> : IRoleStore<T> where T : MongoDbRole
    {
        private IMongoCollection<T> _roles;

        public MongoDbRoleStore(IMongoDatabase mongoDatabase)
        {
            _roles = mongoDatabase.GetCollection<T>(MongoDbCollections.Users);
        }

        public void Dispose()
        {
            //throw new System.NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(T role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(T role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}