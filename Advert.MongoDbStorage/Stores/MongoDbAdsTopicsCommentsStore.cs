using System.Linq;
using System.Threading;
using Advert.MongoDbStorage.AdsTopics;
using Advert.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbAdsTopicsCommentsStore<T> : IAdsTopicsCommentsStore<T> where T : MongoDbAdsTopicComment
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDbAdsTopicsCommentsStore(IMongoDatabase mongoDatabase) : this()
        {
            _mongoDatabase = mongoDatabase;
        }

        private MongoDbAdsTopicsCommentsStore()
        {
            EntityMappings.EnsureMongoDbAdsTopicCommentConfigured();
        }

        public bool AdsTopicCommentsCollectionExists(string collectionName, CancellationToken cancellationToken)
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

        public void DeleteAdsTopicCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.DropCollection(collectionName, cancellationToken);
        }

        public IAdsTopicCommentsCollection<T> GetAdsTopicCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            return new MongoDbAdsTopicCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public IAdsTopicCommentsCollection<T> CreateAdsTopicCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.CreateCollection(collectionName, new CreateCollectionOptions(), cancellationToken);
            return new MongoDbAdsTopicCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public void Dispose()
        {
        }   
    }
}