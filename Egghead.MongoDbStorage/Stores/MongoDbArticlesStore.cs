using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task CreateArticleAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken)
        {    
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindArticleByIdOrDefaultAsync(ObjectId articleId, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticlesByNormalizedEmail(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.CountDocumentsAsync(Builders<T>.Filter.Eq(x => x.NormalizedEmail, email), cancellationToken: cancellationToken);
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

        public async Task<List<T>> FindArticlesAsync(int howManyElements, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),
                Limit = howManyElements
            };
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
        }
        
        public async Task<DeleteResult> DeleteArticleByIdAsync(ObjectId articleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), cancellationToken);        
        }

        public async Task<UpdateResult> UpdateArticleViewsCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), Builders<T>.Update.Set(x => x.ViewsCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
        
        public async Task<UpdateResult> UpdateArticleLikesCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), Builders<T>.Update.Set(x => x.LikesCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<UpdateResult> UpdateArticleDislikesCountByArticleIdAsync(ObjectId articleId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), Builders<T>.Update.Set(x => x.DislikesCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<ReplaceOneResult> ReplaceArticleAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, entity.Id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
    }
}