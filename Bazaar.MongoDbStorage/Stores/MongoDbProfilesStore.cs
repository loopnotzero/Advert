using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Profiles;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbProfilesStore<T> : IProfilesStore<T> where T : IProfile
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbProfilesStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Profiles);          
            //todo: Create indices
        }
        
        private MongoDbProfilesStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbProfile>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapIdMember(x => x.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
//            });
        }

        public async Task CreateProfileAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedName = entity.NormalizedName ?? entity.Name.ToUpper();
            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<ReplaceOneResult> UpdateProfileAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x._id, entity._id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindProfileByIdAsync(ObjectId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, id), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindProfileByNormalizedNameAsync(string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedName, name), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindProfileByNormalizedEmailAsync(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedEmail, email), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindProfileByNormalizedNameOrDefaultAsync(string name, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedName, name), cancellationToken: cancellationToken);
            var result = await cursor.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                return defaultValue;
            }
            return result;
        }
        
        public async Task<T> FindProfileByNormalizedEmailOrDefaultAsync(string email, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedEmail, email), cancellationToken: cancellationToken);          
            var result = await cursor.FirstOrDefaultAsync(cancellationToken);          
            if (result == null)
            {
                return defaultValue;
            }        
            return result;
        }

        public void Dispose()
        {
        }
    }
}