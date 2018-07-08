using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
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
             
        public MongoDbArticlesCommentsStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesComments);          
            //todo: Create indices
        }
        
        private MongoDbArticlesCommentsStore()
        {
            EntityMappings.EnsureMongoDbArticleCommentConfigured();
        }
        
        public void Dispose()
        {
        }
        
        public async Task<T> FindArticleCommentById(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticlesCommentsByArticleIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<OperationResult> CreateArticleCommentAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return OperationResult.Success;
        }
    }
}