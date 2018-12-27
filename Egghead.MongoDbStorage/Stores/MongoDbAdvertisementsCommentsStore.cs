using System.Linq;
using System.Threading;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdvertisementsCommentsStore<T> : IAdvertisementsCommentsStore<T> where T : MongoDbAdvertisementComment
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDbAdvertisementsCommentsStore(IMongoDatabase mongoDatabase) : this()
        {
            _mongoDatabase = mongoDatabase;
        }

        private MongoDbAdvertisementsCommentsStore()
        {
            EntityMappings.EnsureMongoDbAdvertisementCommentConfigured();
        }

        public bool AdvertisementCommentsCollectionExists(string collectionName, CancellationToken cancellationToken)
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

        public void DeleteAdvertisementCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.DropCollection(collectionName, cancellationToken);
        }

        public IAdvertisementCommentsCollection<T> GetAdvertisementCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            return new MongoDbAdvertisementCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public IAdvertisementCommentsCollection<T> CreateAdvertisementCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.CreateCollection(collectionName, new CreateCollectionOptions(), cancellationToken);
            return new MongoDbAdvertisementCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public void Dispose()
        {
        }   
    }
}