using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Common;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbPostsPhotosStore<T> : IPostsPhotosStore<T> where T : IPostPhotos
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

        public Task CreatePostPhotosAsync(T entity, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}