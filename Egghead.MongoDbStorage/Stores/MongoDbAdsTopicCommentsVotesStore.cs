using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.AdsTopics;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdsTopicCommentsVotesStore<T> : IAdsTopicCommentsVotesStore<T> where T : MongoDbAdsTopicCommentVote
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbAdsTopicCommentsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdsTopicCommentsVotes);
            //todo: Create indices
        }

        private MongoDbAdsTopicCommentsVotesStore()
        {
            EntityMappings.EnsureMongoDbAdsTopicCommentVoteConfigured();
        }

        public async Task CreateAdsTopicCommentVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindAdsTopicCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue, CancellationToken cancellationToken)
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

        public async Task<long> CountAdsTopicCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.CommentId, commentId)
            );
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<DeleteResult> DeleteAdsTopicCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.Id, voteId);
            return await _collection.DeleteOneAsync(filter, cancellationToken);
        }
    }
}