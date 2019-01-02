using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbPostsViewCountStore<T> : IPostsViewCountStore<T> where T : MongoDbPostViewsCount
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbPostsViewCountStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostsViews);
            //todo: Create indices
        }

        private MongoDbPostsViewCountStore()
        {
            EntityMappings.EnsureMongoDbPostViewsConfigured();
        }

        public async Task CreatePostViewsCountAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
            
        }

        public async Task<long> CountPostViewsCountByPostIdAsync(ObjectId postId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.PostId, postId);
            return await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }
    }
}