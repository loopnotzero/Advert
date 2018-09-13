using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesViewCountStore<T> : IArticlesViewCountStore<T> where T : MongoDbArticleViewCount
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbArticlesViewCountStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesViews);
            //todo: Create indices
        }

        private MongoDbArticlesViewCountStore()
        {
            EntityMappings.EnsureMongoDbArticleViewsConfigured();
        }

        public async Task<long> CountArticleViewsCountByArticleIdAsync(ObjectId articleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, articleId);
            return await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }
        
        public async Task<IQueryable<T>> AsQueryable(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _collection.AsQueryable();
        }

        public async Task<OperationResult> CreateArticleViewsCountAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            return OperationResult.Success;
        }
    }
}