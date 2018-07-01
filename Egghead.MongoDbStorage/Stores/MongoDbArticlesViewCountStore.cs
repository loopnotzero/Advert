using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
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

        public MongoDbArticlesViewCountStore()
        {
            EntityMappings.EnsureMongoDbArticleViewsConfigured();
        }

        public MongoDbArticlesViewCountStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesViews);          
            //todo: Create indices
        }
        
        public async Task<T> FindArticlesViewCountByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, id);
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticlesViewCountByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, id);
            
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);          
        }

        public async Task<OperationResult> AddArticleViewAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
           
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            
            return OperationResult.Success;
        }
    }
}