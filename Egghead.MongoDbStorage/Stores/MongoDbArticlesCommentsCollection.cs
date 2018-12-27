using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Advertisements;
using Egghead.MongoDbStorage.Advertisements;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdvertisementCommentsCollection<T> : IAdvertisementCommentsCollection<T> where T : MongoDbAdvertisementComment
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbAdvertisementCommentsCollection(IMongoCollection<T> collection)
        {
            _collection = collection;
        }
        
        public async Task CreateAdvertisementCommentAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdvertisementCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<long> EstimatedAdvertisementCommentsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<T>> FindAdvertisementCommentsAsync(int? howManyElements, CancellationToken cancellationToken)
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

        public async Task<List<T>> FindAdvertisementCommentsAsync(int offset, int? howManyElements, CancellationToken cancellationToken)
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

        public async Task<List<T>> FindAdvertisementCommentsAsync(int offset, int? howManyElements, SortDefinition sortDef, CancellationToken cancellationToken)
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

        public async Task<List<T>> FindAdvertisementCommentsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Ascending(field => field.CreatedAt),
            };

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }
        
        public async Task<DeleteResult> DeleteAdvertisementCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), cancellationToken);
        }

        public async Task<ReplaceOneResult> UpdateAdvertisementCommentByIdAsync(ObjectId commentId, T entity, CancellationToken cancellationToken)
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