using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesCommentsStore<T> : IArticlesCommentsStore<T> where T : MongoDbArticleComment
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbArticlesCommentsStore()
        {
            EntityMappings.EnsureMongoDbArticleCommentConfigured();
        }
        
        public MongoDbArticlesCommentsStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesComments);          
            //todo: Create indices
        }
        
        public void Dispose()
        {
        }
        
        public async Task<long> CountArticlesCommentsByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x.ArticleId, id);
           
            return await _collection.CountAsync(filter, cancellationToken: cancellationToken);
        }        
    }
}