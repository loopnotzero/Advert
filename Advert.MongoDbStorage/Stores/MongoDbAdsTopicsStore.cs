using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.MongoDbStorage.AdsTopics;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbAdsTopicsStore<T> : IAdsTopicsStore<T> where T : MongoDbAdsTopic, new()
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbAdsTopicsStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdsTopics);          
            //todo: Create indices
        }
         
        private MongoDbAdsTopicsStore()
        {
            EntityMappings.EnsureMongoDbAdsTopicConfigured();
        }

        public void Dispose()
        {
        }

        public async Task CreateAdsTopicAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedProfileName = entity.NormalizedProfileName ?? entity.ProfileName.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task UpdateAdsTopicAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, entity.Id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdsTopicByIdAsync(ObjectId adsId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindAdsTopicByIdOrDefaultAsync(ObjectId adsId, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> EstimatedAdsTopicsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<long> CountAdsTopicsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.CountDocumentsAsync(Builders<T>.Filter.Eq(x => x.ProfileId, profileId), cancellationToken: cancellationToken);
        }

        public async Task<List<T>> FindAdsTopicsAsync(int? howManyElements, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
           
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),                
            };

            if (howManyElements.HasValue)
            {
                findOptions.Limit = howManyElements;
            }
                       
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.IsDeleted, false), findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindAdsTopicsAsync(int offset, int? howManyElements, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
           
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),                
            };

            findOptions.Skip = offset;

            if (howManyElements.HasValue)
            {
                findOptions.Limit = howManyElements;
            }
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.IsDeleted, false), findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindAdsTopicsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),
            };
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.ProfileId, profileId), findOptions, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);           
        }

        public async Task<List<T>> FindAdsTopicsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
           
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),                
            };

            findOptions.Skip = offset;

            if (howManyElements.HasValue)
            {
                findOptions.Limit = howManyElements;
            }
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Regex(x => x.Title, new BsonRegularExpression(keyword, "-i")), findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<UpdateResult> DeleteAdsTopicByIdAsync(ObjectId adsId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), Builders<T>.Update.Set(x => x.IsDeleted, true), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);      
        }

        public async Task<UpdateResult> UpdateAdsTopicViewsCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), Builders<T>.Update.Set(x => x.ViewsCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
        
        public async Task<UpdateResult> UpdateAdsTopicLikesCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), Builders<T>.Update.Set(x => x.LikesCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        } 

        public async Task<ReplaceOneResult> ReplaceAdsTopicAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, entity.Id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
    }
}