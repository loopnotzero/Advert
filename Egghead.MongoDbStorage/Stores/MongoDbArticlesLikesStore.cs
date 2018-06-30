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
    public class MongoDbArticlesLikesStore<T> : IArticlesLikesStore<T> where T : MongoDbArticleLike
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbArticlesLikesStore()
        {
            EntityMappings.EnsureMongoDbArticleLikeConfigured();
        }

        public MongoDbArticlesLikesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesLikes);          
            //todo: Create indices
        }

        public async Task<T> FindArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
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

        public async Task<T> FindArticlesUnlikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.UnLike));
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticlesLikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Like));
           
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<long> CountArticlesUnlikesByArticleIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.UnLike));
            
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);
        }
    }
}