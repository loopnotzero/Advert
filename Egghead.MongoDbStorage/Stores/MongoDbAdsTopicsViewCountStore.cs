using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.AdsTopics;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdsTopicsViewCountStore<T> : IAdsTopicsViewCountStore<T> where T : MongoDbAdsTopicViewsCount
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbAdsTopicsViewCountStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdsTopicsViews);
            //todo: Create indices
        }

        private MongoDbAdsTopicsViewCountStore()
        {
            EntityMappings.EnsureMongoDbAdsTopicViewsConfigured();
        }

        public async Task CreateAdsTopicViewsCountAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdsTopicViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
            
        }

        public async Task<long> CountAdsTopicViewsCountByAdsIdAsync(ObjectId adsId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.AdsId, adsId);
            return await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }
    }
}