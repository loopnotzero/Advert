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
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Subjects);          
            //todo: Create indices
        }
               
        public void Dispose()
        {
        }

        public Task SetNormalizedTitleAsync(T subject, string normalizedTitle, CancellationToken cancellationToken)
        {
            if (subject == null)
            {
                throw new NullReferenceException();
            }         
            
            cancellationToken.ThrowIfCancellationRequested();
            
            subject.NormalizedTitle = normalizedTitle ?? subject.Title.ToUpper();
            
            return Task.FromResult<object>(null);
        }

        public async Task<T> FindSubjectByIdAsync(string subjectId, CancellationToken cancellationToken)
        {
            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, subjectId), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindSubjectByTitleAsync(string normalizedTitle, CancellationToken cancellationToken)
        {
            if (normalizedTitle == null)
            {
                throw new ArgumentNullException(nameof(normalizedTitle));
            }
                    
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.NormalizedTitle, normalizedTitle), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<T>> GetSubjects(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, cancellationToken: cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<IdentityResult> CreateSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(subject, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            if (subject == null)
            {
                throw new ArgumentNullException();
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, subject.Id), subject, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, subject.Id), cancellationToken);
            
            return IdentityResult.Success;
        }
    }
}