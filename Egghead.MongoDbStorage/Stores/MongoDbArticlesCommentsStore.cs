using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesCommentsStore<T> : IArticlesCommentsStore<T> where T : MongoDbArticleComment
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDbArticlesCommentsStore(IMongoDatabase mongoDatabase) : this()
        {
            _mongoDatabase = mongoDatabase;
        }

        private MongoDbArticlesCommentsStore()
        {
            EntityMappings.EnsureMongoDbArticleCommentConfigured();
        }
        
        public OperationResult DeleteArticleCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.DropCollection(collectionName, cancellationToken);
            return OperationResult.Success;
        }

        public IArticleCommentsCollection<T> GetArticleCommentsCollection(string collectionName)
        {
            var collection = _mongoDatabase.GetCollection<T>(collectionName, new MongoCollectionSettings
            {
                //todo: maybe some options?
            });
            
            return new MongoDbArticleCommentsCollection<T>(collection);
        }

        public IArticleCommentsCollection<T> CreateArticleCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.CreateCollection(collectionName, new CreateCollectionOptions
            {
                //todo: maybe some options?
            }, cancellationToken);

            var collection = _mongoDatabase.GetCollection<T>(collectionName, new MongoCollectionSettings
            {
                //todo: maybe some options?
            });
            
            return new MongoDbArticleCommentsCollection<T>(collection);
        }

        public void Dispose()
        {
        }   
    }
}