using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Advertisements;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdvertisementsVotesStore<T> : IAdvertisementsVotesStore<T> where T : MongoDbAdvertisementVote
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbAdvertisementsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdvertisementsVotes);          
            //todo: Create indices
        }

        private MongoDbAdvertisementsVotesStore()
        {
            EntityMappings.EnsureMongoDbAdvertismentVoteConfigured();
        }

        public async Task CreateAdvertisementVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdvertisementVoteByNormalizedEmailAsync(ObjectId adsId, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.AdsId, adsId), Builders<T>.Filter.Eq(x => x.NormalizedEmail, email));
            var cursor = await _collection.FindAsync(filter, new FindOptions<T>
            {
                
            }, cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountAdvertisementVotesByVoteTypeAsync(ObjectId adsId, VoteType voteType, CancellationToken cancellationToken)
        {      
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.AdsId, adsId), Builders<T>.Filter.Eq(x => x.VoteType, voteType)
                );          
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }
       
        public async Task<DeleteResult> DeleteAdvertisementVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, voteId), cancellationToken);            
        }
    }
}