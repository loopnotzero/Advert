using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbPostsPhotosStore<T> : IPostsPhotosStore<T> where T : IPostPhoto
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbPostsPhotosStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostsPhotos);
        }

        private MongoDbPostsPhotosStore()
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
        
        public Task CreatePostPhotoAsync(T entity, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task DeletePostPhotoById(ObjectId photoId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task DeletePostPhotosByPostId(ObjectId postId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task DeletePostPhotoByFileName(string fileName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<T>> GetPostPhotosByPostIdAsync(ObjectId postId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}