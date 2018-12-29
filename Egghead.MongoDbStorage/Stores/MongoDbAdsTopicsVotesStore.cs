using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.AdsTopics;
using Egghead.MongoDbStorage.AdsTopics;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdsTopicsVotesStore<T> : IAdsTopicsVotesStore<T> where T : MongoDbAdsTopicVote
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbAdsTopicsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdsTopicsVotes);          
            //todo: Create indices
        }

        private MongoDbAdsTopicsVotesStore()
        {
            EntityMappings.EnsureMongoDbAdsTopicVoteConfigured();
        }

        public async Task CreateAdsTopicVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdsTopicVoteByNormalizedEmailAsync(ObjectId adsId, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.AdsId, adsId), Builders<T>.Filter.Eq(x => x.NormalizedEmail, email));
            var cursor = await _collection.FindAsync(filter, new FindOptions<T>
            {
                
            }, cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountAdsTopicVotesByVoteTypeAsync(ObjectId adsId, VoteType voteType, CancellationToken cancellationToken)
        {      
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.AdsId, adsId), Builders<T>.Filter.Eq(x => x.VoteType, voteType)
                );          
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }
       
        public async Task<DeleteResult> DeleteAdsTopicVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, voteId), cancellationToken);            
        }
    }
}