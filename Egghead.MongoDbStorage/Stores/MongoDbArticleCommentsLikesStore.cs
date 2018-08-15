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
    public class MongoDbArticleCommentsLikesStore<T> : IArticleCommentsLikesStore<T> where T : MongoDbArticleCommentLike
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbArticleCommentsLikesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesLikes);          
            //todo: Create indices
        }

        private MongoDbArticleCommentsLikesStore()
        {
            EntityMappings.EnsureMongoDbArticleLikeConfigured();
        }

        public async Task<T> FindArticleCommentLikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.ArticleId, articleId),
                Builders<T>.Filter.Eq(x => x.CommentId, commentId),
                Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Like)
                );
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindArticleCommentDislikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.ArticleId, articleId),
                Builders<T>.Filter.Eq(x => x.CommentId, commentId),
                Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Dislike)
            );
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticleCommentLikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.ArticleId, articleId),
                Builders<T>.Filter.Eq(x => x.CommentId, commentId),
                Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Like)
            );
            
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<long> CountArticleCommentDislikesByArticleCommentIdAsync(string articleId, string commentId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.ArticleId, articleId),
                Builders<T>.Filter.Eq(x => x.CommentId, commentId),
                Builders<T>.Filter.Eq(x => x.LikeType, LikeType.Dislike)
            );
            
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<OperationResult> SetArticleCommentLikeAsync(T entity, CancellationToken cancellationToken)
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