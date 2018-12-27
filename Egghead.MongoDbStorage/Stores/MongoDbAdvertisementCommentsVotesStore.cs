using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdvertisementCommentsVotesStore<T> : IAdvertisementCommentsVotesStore<T> where T : MongoDbAdvertisementCommentVote
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbAdvertisementCommentsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdvertisementCommentsVotes);
            //todo: Create indices
        }

        private MongoDbAdvertisementCommentsVotesStore()
        {
            EntityMappings.EnsureMongoDbAdvertisementCommentVoteConfigured();
        }

        public async Task CreateAdvertisementCommentVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdvertisementCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.CommentId, commentId),
                Builders<T>.Filter.Eq(x => x.ProfileId, profileId)
            );
            
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

            var commentVote = await cursor.FirstOrDefaultAsync(cancellationToken);

            return commentVote ?? defaultValue;
        }

        public async Task<long> CountAdvertisementCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.CommentId, commentId)
            );
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<DeleteResult> DeleteAdvertisementCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.Id, voteId);
            return await _collection.DeleteOneAsync(filter, cancellationToken);
        }
    }
}