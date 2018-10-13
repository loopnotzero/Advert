using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using Egghead.MongoDbStorage.Profiles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbProfilesPhotosStore<T> : IProfilesPhotosStore<T> where T : MongoDbProfilePhoto
    {
        private readonly IMongoCollection<T> _collection;
      
        public MongoDbProfilesPhotosStore()
        {
            EntityMappings.EnsureMongoDbProfileImageConfigured();
        }
        
        public MongoDbProfilesPhotosStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ProfilesImages);          
            //todo: Create indices
        }
      
        public void Dispose()
        {
            
        }
        
        public async Task CreateProfilePhotoAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
 
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> GetProfilePhotoById(ObjectId imageId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, imageId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }


        public async Task<T> GetProfilePhotoByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.ProfileId, profileId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }
    }
}