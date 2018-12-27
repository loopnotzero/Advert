using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdvertisementsStore<T> : IAdvertisementsStore<T> where T : MongoDbAdvertisement, new()
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbAdvertisementsStore(IMongoDatabase mongoDatabase) : this()
        {          
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.Advertisements);          
            //todo: Create indices
        }
         
        private MongoDbAdvertisementsStore()
        {
            EntityMappings.EnsureMongoDbAdvertisementConfigured();
        }

        public void Dispose()
        {
        }

        public async Task CreateAdvertisementAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedProfileName = entity.NormalizedProfileName ?? entity.ProfileName.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task UpdateAdvertisementAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, entity.Id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdvertisementByIdAsync(ObjectId adsId, CancellationToken cancellationToken)
        {    
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), cancellationToken: cancellationToken);
            return await cursor.FirstAsync(cancellationToken);
        }

        public async Task<T> FindAdvertisementByIdOrDefaultAsync(ObjectId adsId, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> EstimatedAdvertisementsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<long> CountAdvertisementsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.CountDocumentsAsync(Builders<T>.Filter.Eq(x => x.ProfileId, profileId), cancellationToken: cancellationToken);
        }

        public async Task<List<T>> FindAdvertisementsAsync(int? howManyElements, CancellationToken cancellationToken)
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
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindAdvertisementsAsync(int offset, int? howManyElements, CancellationToken cancellationToken)
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
            
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken: cancellationToken);
           
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> FindAdvertisementsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var findOptions = new FindOptions<T>
            {
                Sort = Builders<T>.Sort.Descending(field => field.CreatedAt),
            };
            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.ProfileId, profileId), findOptions, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);           
        }

        public async Task<List<T>> FindAdvertisementsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword, CancellationToken cancellationToken)
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

        public async Task<DeleteResult> DeleteAdvertisementByIdAsync(ObjectId adsId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), cancellationToken);        
        }

        public async Task<UpdateResult> UpdateAdvertisementViewsCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), Builders<T>.Update.Set(x => x.ViewsCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
        
        public async Task<UpdateResult> UpdateAdvertisementLikesCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Id, adsId), Builders<T>.Update.Set(x => x.LikesCount, count), new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        } 

        public async Task<ReplaceOneResult> ReplaceAdvertisementAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, entity.Id), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }
    }
}