using System;
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
        }
        
        private MongoDbProfilesStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbProfile>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapIdMember(x => x.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
//            });
        }
        
        public string CreateDefaultIndexes()
        {
            return _collection.Indexes.CreateOne(
                new CreateIndexModel<T>(Builders<T>.IndexKeys.Hashed(x => x.IdentityName))
            );
        }

        public async Task CreateProfileAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
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

        public async Task<T> FindProfileByIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, profileId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindProfileByIdOrDefaultAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, profileId), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindProfileByNormalizedName(string profileName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedName, profileName), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindProfileByNormalizedNameOrDefault(string profileName, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedName, profileName), cancellationToken: cancellationToken);          
            var profile = await cursor.FirstOrDefaultAsync(cancellationToken);          
            return profile == null ? defaultValue : profile;
        }

        public async Task<T> FindProfileByIdentityNameAsync(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.IdentityName, email), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindProfileByIdentityNameOrDefaultAsync(string email, T defaultValue,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.IdentityName, email), cancellationToken: cancellationToken);          
            var profile = await cursor.FirstOrDefaultAsync(cancellationToken);          
            return profile == null ? defaultValue : profile;
        }
        
        public void Dispose()
        {
        }
    }
}