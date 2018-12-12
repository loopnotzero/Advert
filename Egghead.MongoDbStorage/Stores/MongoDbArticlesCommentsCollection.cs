using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.Articles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticleCommentsCollection<T> : IArticleCommentsCollection<T> where T : MongoDbArticleComment
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbArticleCommentsCollection(IMongoCollection<T> collection)
        {
            _collection = collection;
        }
        
        public async Task CreateArticleCommentAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindArticleCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<long> EstimatedArticleCommentsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<T>> FindArticleCommentsAsync(int? howManyElements, CancellationToken cancellationToken)
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
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken);
            
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindArticleCommentsAsync(int offset, int? howManyElements, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Ascending(field => field.CreatedAt),
            };

            findOptions.Skip = offset;

            if (howManyElements.HasValue)
            {
                findOptions.Limit = howManyElements;
            }
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken);
            
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindArticleCommentsAsync(int offset, int? howManyElements, SortDefinition sortDef, CancellationToken cancellationToken)
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
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken);
            
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindArticleCommentsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Ascending(field => field.CreatedAt),
            };

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }
        
        public async Task<DeleteResult> DeleteArticleCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), cancellationToken);
        }

        public async Task<ReplaceOneResult> UpdateArticleCommentByIdAsync(ObjectId commentId, T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}