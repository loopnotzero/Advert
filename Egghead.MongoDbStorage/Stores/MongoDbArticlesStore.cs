using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Entities;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Mappings;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesStore<T> : IArticlesStore<T> where T : MongoDbArticle
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbArticlesStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Articles);          
            //todo: Create indices
        }
         
        private MongoDbArticlesStore()
        {
            EntityMappings.EnsureMongoDbArticleConfigured();
        }

        public void Dispose()
        {
        }

        public Task SetNormalizedTitleAsync(T entity, string normalizedTitle, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new NullReferenceException();
            }         
            
            cancellationToken.ThrowIfCancellationRequested();
            
            entity.NormalizedTitle = normalizedTitle ?? entity.Title.ToUpper();
            
            return Task.FromResult<object>(null);
        }

        public async Task<T> FindArticleByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindArticleByTitleAsync(string title, CancellationToken cancellationToken)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<T>> GetArticles(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var findOptions = new FindOptions<T> {Sort = Builders<T>.Sort.Descending(field => field.CreatedAt)};

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken: cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<OperationResult> CreateArticleAsync(T entity, CancellationToken cancellationToken)
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

        public async Task<OperationResult> UpdateArticleByIdAsync(string id, T entity, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateArticleByTitleAsync(string title, T entity, CancellationToken cancellationToken)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteArticleByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, id), cancellationToken);
            
            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteArticleByTitleAsync(string title, CancellationToken cancellationToken)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), cancellationToken);
            
            return OperationResult.Success;
        }
    }
}