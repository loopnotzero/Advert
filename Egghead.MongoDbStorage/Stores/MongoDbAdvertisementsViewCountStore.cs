using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdvertisementsViewCountStore<T> : IAdvertisementsViewCountStore<T> where T : MongoDbAdvertisementViewsCount
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbAdvertisementsViewCountStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdvertisementsViews);
            //todo: Create indices
        }

        private MongoDbAdvertisementsViewCountStore()
        {
            EntityMappings.EnsureMongoDbAdvertisementViewsConfigured();
        }

        public async Task CreateAdvertisementViewsCountAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdvertisementViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
            
        }

        public async Task<long> CountAdvertisementViewsCountByAdsIdAsync(ObjectId adsId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.AdsId, adsId);
            return await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }
    }
}