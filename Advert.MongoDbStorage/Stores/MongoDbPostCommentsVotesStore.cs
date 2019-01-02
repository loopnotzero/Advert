using System.Threading;
using System.Threading.Tasks;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbPostCommentsVotesStore<T> : IPostCommentsVotesStore<T> where T : MongoDbPostCommentVote
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbPostCommentsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostCommentsVotes);
            //todo: Create indices
        }

        private MongoDbPostCommentsVotesStore()
        {
            EntityMappings.EnsureMongoDbPostCommentVoteConfigured();
        }

        public async Task CreatePostCommentVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue, CancellationToken cancellationToken)
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

        public async Task<long> CountPostCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.CommentId, commentId)
            );
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<DeleteResult> DeletePostCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.Id, voteId);
            return await _collection.DeleteOneAsync(filter, cancellationToken);
        }
    }
}