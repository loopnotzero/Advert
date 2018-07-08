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

        public async Task<OperationResult> AddArticleLikeAsync(T entity, CancellationToken cancellationToken)
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

        public async Task<T> FindArticlesLikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, id), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Like));
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindArticlesDislikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, id), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Dislike));
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticlesLikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, id), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Like));
           
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<long> CountArticlesDislikesByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.ArticleId, id), Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Dislike));
            
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);
        }
    }
}