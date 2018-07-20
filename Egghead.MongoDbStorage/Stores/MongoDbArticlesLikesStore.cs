using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesLikesStore<T> : IArticlesLikesStore<T> where T : MongoDbArticleLike
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbArticlesLikesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesLikes);          
            //todo: Create indices
        }

        private MongoDbArticlesLikesStore()
        {
            EntityMappings.EnsureMongoDbArticleLikeConfigured();
        }

        public async Task<OperationResult> SetArticleLikeAsync(T entity, CancellationToken cancellationToken)
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

        public async Task<T> FindArticleLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Like));
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindArticleDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Dislike));
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticleLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Like));
           
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<long> CountArticleDislikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Dislike));
            
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);
        }
    }
}