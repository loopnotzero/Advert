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
    public class MongoDbProfilesImagesStore<T> : IProfilesImagesStore<T> where T : MongoDbProfileImage
    {
        private readonly IMongoCollection<T> _collection;

        

        public MongoDbProfilesImagesStore()
        {
            EntityMappings.EnsureMongoDbProfileImageConfigured();
        }
        
        public MongoDbProfilesImagesStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ProfilesImages);          
            //todo: Create indices
        }
      
        public void Dispose()
        {
            
        }
        
        public Task<T> GetProfileImageByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResult> CreateProfileImageAsync(T entity, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}