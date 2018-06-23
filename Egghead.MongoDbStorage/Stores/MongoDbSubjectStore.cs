using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Mappings;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbSubjectStore<T> : ISubjectStore<T> where T : MongoDbSubject
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbSubjectStore()
        {
            RegisterMappings.EnsureConfigure();
        }
        
        public MongoDbSubjectStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Subjects);          
            //todo: Create indices
        }
               
        public void Dispose()
        {
        }

        public Task SetNormalizedTitleAsync(T subject, string normalizedTitle, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> FindSubjectByIdAsync(string subjectId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> FindSubjectByTitleAsync(string title, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> CreateSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> DeleteSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}