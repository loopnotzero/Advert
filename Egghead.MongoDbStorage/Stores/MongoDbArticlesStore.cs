using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public MongoDbArticlesStore()
        {
            EntityMappings.EnsureMongoDbArticlesConfigured();
        }
        
        public MongoDbArticlesStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Articles);          
            //todo: Create indices
        }
               
        public void Dispose()
        {
        }

        public Task SetNormalizedTitleAsync(T article, string normalizedTitle, CancellationToken cancellationToken)
        {
            if (article == null)
            {
                throw new NullReferenceException();
            }         
            
            cancellationToken.ThrowIfCancellationRequested();
            
            article.NormalizedTitle = normalizedTitle ?? article.Title.ToUpper();
            
            return Task.FromResult<object>(null);
        }

        public async Task<T> FindArticleByIdAsync(string objectId, CancellationToken cancellationToken)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, objectId), cancellationToken: cancellationToken);

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

        public async Task<IdentityResult> CreateArticleAsync(T article, CancellationToken cancellationToken)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(article, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateArticleByIdAsync(string objectId, T article, CancellationToken cancellationToken)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, objectId), article, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateArticleByTitleAsync(string title, T article, CancellationToken cancellationToken)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), article, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteArticleByIdAsync(string objectId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, objectId), cancellationToken);
            
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteArticleByTitleAsync(string title, CancellationToken cancellationToken)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, title), cancellationToken);
            
            return IdentityResult.Success;
        }
    }
}