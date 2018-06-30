using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesViewsStore<T> : IArticlesViewsStore<T> where T : MongoDbArticleViews
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbArticlesViewsStore()
        {
            EntityMappings.EnsureMongoDbArticleViewsConfigured();
        }

        public MongoDbArticlesViewsStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesViews);          
            //todo: Create indices
        }
        
        public async Task<T> FindArticlesViewsByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, articleId);
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticlesViewsByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, articleId);
            
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);          
        }
    }
}