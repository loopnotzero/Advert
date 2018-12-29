using System.Threading;
using System.Threading.Tasks;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Mappings;
using Advert.MongoDbStorage.Profiles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
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

        public async Task UpdateProfileAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, entity.Id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindProfileByIdAsync(ObjectId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken: cancellationToken);
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
            return await cursor.FirstOrDefaultAsync(cancellationToken) ?? defaultValue;
        }
        
        public async Task<T> FindProfileByNormalizedEmailOrDefaultAsync(string email, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedEmail, email), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken) ?? defaultValue;
        }
    }
}