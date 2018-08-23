using System;
using System.Collections.Generic;
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
    public class MongoDbArticlesStore<T> : IArticlesStore<T> where T : MongoDbArticle
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbArticlesStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Articles);          
            //todo: Create indices
        }
         
        private MongoDbArticlesStore()
        {
            EntityMappings.EnsureMongoDbArticleConfigured();
        }

        public void Dispose()
        {
        }

        public Task SetNormalizedTitleAsync(T entity, string normalizedTitle, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();           
            entity.NormalizedTitle = normalizedTitle ?? entity.Title.ToUpper();           
            return Task.FromResult<object>(null);
        }

        public async Task<T> FindArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken)
        {    
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindArticleByTitleAsync(string title, CancellationToken cancellationToken)
        {                    
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticlesByWhoNormalizedAsync(string byWhoNormalized, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.byWhoNormalized, byWhoNormalized);
            var articlesCount = await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
            return articlesCount;
        }

        public async Task<List<T>> FindArticlesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),                
            };
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindArticlesAsync(int limit, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),
                Limit = limit
            };
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<OperationResult> CreateArticleAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateArticleByIdAsync(ObjectId articleId, T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateArticleByTitleAsync(string title, T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), cancellationToken);        
            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteArticleByTitleAsync(string title, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), cancellationToken);          
            return OperationResult.Success;
        }
    }
}