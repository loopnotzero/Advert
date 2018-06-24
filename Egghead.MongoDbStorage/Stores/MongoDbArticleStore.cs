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
    public class MongoDbArticleStore<T> : IArticleStore<T> where T : MongoDbArticle
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbArticleStore()
        {
            EntityMappings.EnsureMongoDbArticleConfigured();
        }
        
        public MongoDbArticleStore(IMongoDatabase mongoDatabase) : this()
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

        public async Task<T> FindArticleByIdAsync(string articleId, CancellationToken cancellationToken)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, articleId), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindArticleByTitleAsync(string normalizedTitle, CancellationToken cancellationToken)
        {
            if (normalizedTitle == null)
            {
                throw new ArgumentNullException(nameof(normalizedTitle));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, normalizedTitle), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<T>> GetArticles(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, cancellationToken: cancellationToken);

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

        public async Task<IdentityResult> UpdateArticleAsync(T article, CancellationToken cancellationToken)
        {
            if (article == null)
            {
                throw new ArgumentNullException();
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, article.Id), article, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteArticleAsync(T article, CancellationToken cancellationToken)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, article.Id), cancellationToken);
            
            return IdentityResult.Success;
        }
    }
}