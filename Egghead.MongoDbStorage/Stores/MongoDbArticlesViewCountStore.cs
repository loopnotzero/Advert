﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesViewCountStore<T> : IArticlesViewCountStore<T> where T : MongoDbArticleViewsCount
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

        public async Task CreateArticleViewsCountAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindArticleViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
            
        }

        public async Task<long> CountArticleViewsCountByArticleIdAsync(ObjectId articleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, articleId);
            return await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }
    }
}