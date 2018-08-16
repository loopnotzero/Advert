using System;
using System.Linq;
using System.Threading;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
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

        public bool ArticleCommentsCollectionExists(string collectionName, CancellationToken cancellationToken)
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

        public OperationResult DeleteArticleCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.DropCollection(collectionName, cancellationToken);        
            return OperationResult.Success; 
        }

        public IArticleCommentsCollection<T> GetArticleCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            return new MongoDbArticleCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public IArticleCommentsCollection<T> CreateArticleCommentsCollection(string collectionName, CancellationToken cancellationToken)
        {
            _mongoDatabase.CreateCollection(collectionName, new CreateCollectionOptions(), cancellationToken);
            return new MongoDbArticleCommentsCollection<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public void Dispose()
        {
        }   
    }
}