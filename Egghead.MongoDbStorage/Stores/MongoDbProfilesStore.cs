using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using Egghead.MongoDbStorage.Profiles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbProfilesStore<T> : IProfilesStore<T> where T : MongoDbProfile
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbProfilesStore()
        {
            EntityMappings.EnsureMongoDbProfileConfigured();
        }
        
        public MongoDbProfilesStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Profiles);          
            //todo: Create indices
        }
        
        public void Dispose()
        {
            
        }

        public Task<T> GetProfileByIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResult> CreateProfileAsync(T entity, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}