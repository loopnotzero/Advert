using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
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

        public async Task<long> CountArticleViewsCountByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, articleId);
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
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
        
        public async Task<IEnumerable<string>> FindArticlesPopularOnEgghead(int limit, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var aggregation = _collection.AsQueryable().GroupBy(x => x.ArticleId)
                .Select(x => new MongoDbTopArticle
                {
                    ArticleId = x.Key,
                    ViewsCount = x.Count()
                })
                .OrderByDescending(x => x.ViewsCount)
                .Take(limit);

            return aggregation.Select(x => x.ArticleId);
        }
    }
}