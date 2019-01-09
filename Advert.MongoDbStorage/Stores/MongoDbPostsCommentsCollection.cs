using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.MongoDbStorage.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbPostsCommentsCollection<T> : IPostCommentsCollection<T> where T : IPostComment
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbPostsCommentsCollection(IMongoCollection<T> collection)
        {
            _collection = collection;
        }
        
        public async Task CreatePostCommentAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();        
            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x._id, commentId), Builders<T>.Filter.Eq(x => x.IsDeleted, false));         
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<long> CountPostCommentsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            return await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }

        public async Task<long> EstimatedPostCommentsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<T>> FindPostCommentsAsync(int? howManyElements, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Ascending(field => field.CreatedAt),
            };

            if (howManyElements.HasValue)
            {
                findOptions.Limit = howManyElements;
            }
            
            var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            
            var cursor = await _collection.FindAsync(filter, findOptions, cancellationToken);
            
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindPostCommentsAsync(int offset, int? howManyElements, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var findOptions = new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(field => field.CreatedAt), Skip = offset,};

            if (howManyElements.HasValue)
            {
                findOptions.Limit = howManyElements;
            }
            
            var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            
            var cursor = await _collection.FindAsync(filter, findOptions, cancellationToken);
            
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindPostCommentsAsync(int offset, int? howManyElements, SortDefinition sortDef, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var findOptions = new FindOptions<T>
            {
                Sort = sortDef == SortDefinition.Ascending ? Builders<T>.Sort.Ascending(field => field.CreatedAt) : Builders<T>.Sort.Descending(field => field.CreatedAt), 
                Skip = offset,
            };

            if (howManyElements.HasValue)
            {
                findOptions.Limit = howManyElements;
            }
            
            var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            
            var cursor = await _collection.FindAsync(filter, findOptions, cancellationToken);
            
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindPostCommentsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Ascending(field => field.CreatedAt),
            };

            var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);
            
            var cursor = await _collection.FindAsync(filter, findOptions, cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<UpdateResult> DeletePostCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Or(
                Builders<T>.Filter.Eq(x => x._id, commentId), Builders<T>.Filter.Eq(x => x.ReplyTo, commentId)
            );

            return await _collection.UpdateManyAsync(filter, Builders<T>.Update.Set(x => x.IsDeleted, true).Set(x => x.DeletedAt, DateTime.UtcNow),
                new UpdateOptions
                {
                    BypassDocumentValidation = false
                }, cancellationToken);         
        }

        public async Task<ReplaceOneResult> ReplacePostCommentAsync(ObjectId commentId, T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            entity.ChangedAt = DateTime.UtcNow;
            
            return await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x._id, commentId), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}