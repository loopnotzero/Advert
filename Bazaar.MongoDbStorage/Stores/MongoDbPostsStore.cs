using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbPostsStore<T> : IPostsStore<T> where T : IPost
    {
        private readonly IMongoCollection<T> _collection;
           
        public MongoDbPostsStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Posts);          
        }

        private MongoDbPostsStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbPost>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapCreator(x => new MongoDbPost());
//            });
        }

        public string CreateDefaultIndexes()
        {
            return _collection.Indexes.CreateOne(
                new CreateIndexModel<T>(Builders<T>.IndexKeys.Hashed(x => x.IdentityName))
            );
        }
        
        public async Task CreatePostAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostByIdAsync(ObjectId postId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, postId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindPostByIdOrDefaultAsync(ObjectId postId, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, postId), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }
        
        public async Task<long> EstimatedPostsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<long> CountPostsByIdentityNameAsync(string identityName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.CountDocumentsAsync(Builders<T>.Filter.Eq(x => x.IdentityName, identityName), cancellationToken: cancellationToken);
        }

        public async Task<List<T>> FindPostsAsync(List<ObjectId> postIds, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x._id, postIds[0]);

            filter = postIds.Skip(1).Aggregate(filter, (current, postId) => current | Builders<T>.Filter.Eq(x => x._id, postId));

            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),                
            };
                  
            var cursor = await _collection.FindAsync(filter, findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindPostsAsync(int offset, int? limit, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt), 
                Skip = offset,
            };

            if (limit.HasValue)
            {
                findOptions.Limit = limit;
            }
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.IsDeleted, false), findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindPostsByKeywordAsync(int offset, int? limit, string keyword, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
           
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),                
            };

            findOptions.Skip = offset;

            if (limit.HasValue)
            {
                findOptions.Limit = limit;
            }
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Regex(x => x.Title, new BsonRegularExpression(keyword, "-i")), findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindPostsByIdentityNameAsync(string identityName, int offset, int? limit, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var findOptions = new FindOptions<T>{Sort = Builders<T>.Sort.Descending(field => field.CreatedAt), Skip = offset};

            if (limit.HasValue)
            {
                findOptions.Limit = limit;
            }

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.IdentityName, identityName), findOptions, cancellationToken: cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }
        
        public async Task<List<T>> FindHiddenPostsByIdentityNameAsync(string identityName, int offset, int? limit,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.IdentityName, identityName),
                Builders<T>.Filter.Eq(x => x.Hidden, true));

            var findOptions = new FindOptions<T> {Sort = Builders<T>.Sort.Descending(field => field.CreatedAt), Skip = offset};

            if (limit.HasValue)
            {
                findOptions.Limit = limit;
            }
            
            var cursor = await _collection.FindAsync(filter, findOptions, cancellationToken: cancellationToken);
            
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<UpdateResult> DeletePostByIdAsync(ObjectId postId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(
                Builders<T>.Filter.Eq(x => x._id, postId), 
                Builders<T>.Update.Set(x => x.IsDeleted, true).Set(x => x.DeletedAt, DateTime.UtcNow), 
                new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);      
        }

        public async Task<UpdateResult> UpdatePostViewsCountByPostIdAsync(ObjectId postId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x._id, postId), Builders<T>.Update.Set(x => x.ViewsCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
        
        public async Task<UpdateResult> UpdatePostLikesCountByPostIdAsync(ObjectId postId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x._id, postId), Builders<T>.Update.Set(x => x.LikesCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        } 

        public async Task<ReplaceOneResult> UpdatePostAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.UpdatedAt = DateTime.UtcNow;
            
            return await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x._id, entity._id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
        
        public void Dispose()
        {
        }
    }
}