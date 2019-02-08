using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Profiles;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbProfilesPhotosStore<T> : IProfilesPhotosStore<T> where T : IProfilePhoto
    {
        private readonly IMongoCollection<T> _collection;
      
        public MongoDbProfilesPhotosStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbProfileImage>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapIdMember(x => x.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
//            });
        }
        
        public MongoDbProfilesPhotosStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ProfilesPhotos);          
            //todo: Create indices
        }
      
        public void Dispose()
        {
            
        }
        
        public async Task CreateProfileImageAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
 
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> GetProfileImageById(ObjectId imageId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, imageId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }


        public async Task<T> GetProfileImageByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.ProfileId, profileId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }
    }
}