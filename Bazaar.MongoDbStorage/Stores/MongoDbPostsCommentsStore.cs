using System.Linq;
using System.Threading;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbPostsCommentsStore<T> : IPostsCommentsStore<T> where T : IPostComment
    {
        private readonly IMongoDatabase _mongoDatabase;
   
        public MongoDbPostsCommentsStore(IMongoDatabase mongoDatabase) : this()
        {
            _mongoDatabase = mongoDatabase;
        }
        
        private MongoDbPostsCommentsStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbPostComment>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapIdMember(x => x.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
//            });
        }
        
        public bool PostCommentsCollectionExists(string collectionName, CancellationToken cancellationToken)
        {
            //todo: Improve filter 
            var cursor = _mongoDatabase.ListCollectionNames(new ListCollectionNamesOptions 
            { 
                Filter = FilterDefinition<BsonDocument>.Empty 
            }, cancellationToken); 
 
            while (cursor.MoveNext()) 
            { 
                if (cursor.Current.Any(x => x.Equals(collectionName))) 
                { 
                    return true; 
                } 
            } 
 
            return false; 
        }

        public void DeletePostCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.DropCollection(collectionName, cancellationToken);
        }

        public IPostCommentsCollection<T> GetPostCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            return new MongoDbPostsCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public IPostCommentsCollection<T> CreatePostCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.CreateCollection(collectionName, new CreateCollectionOptions(), cancellationToken);
            return new MongoDbPostsCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public void Dispose()
        {
        }   
    }
}