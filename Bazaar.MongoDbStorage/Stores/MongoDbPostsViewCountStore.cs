﻿using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Posts;
using Bazaar.MongoDbStorage.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbPostsViewCountStore<T> : IPostsViewCountStore<T> where T : IPostViewsCount
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbPostsViewCountStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostsViews);
            //todo: Create indices
        }

        private MongoDbPostsViewCountStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbPostViewsCount>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapIdMember(x => x.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
//            });
        }

        public async Task CreatePostViewsCountAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x._id, id), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
            
        }

        public async Task<long> CountPostViewsCountByPostIdAsync(ObjectId postId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.PostId, postId);
            return await _collection.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}