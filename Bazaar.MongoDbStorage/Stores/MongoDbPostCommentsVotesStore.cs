using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Bazaar.MongoDbStorage.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.MongoDbStorage.Stores
{
    public class MongoDbPostCommentsVotesStore<T> : IPostCommentsVotesStore<T> where T : IPostCommentVote
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbPostCommentsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostCommentsVotes);
        }
        
        private MongoDbPostCommentsVotesStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbPostCommentVote>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapIdMember(x => x.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
//            });
        }
        
        public string CreateDefaultIndexes()
        {
            return _collection.Indexes.CreateOne(
                new CreateIndexModel<T>(Builders<T>.IndexKeys.Hashed(x => x.IdentityName))
            );
        }

        public async Task CreatePostCommentVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostCommentVoteByCommentIdOwnedByOrDefaultAsync(ObjectId commentId,
            string identityName, T defaultValue,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();          
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.CommentId, commentId),
                Builders<T>.Filter.Eq(x => x.IdentityName, identityName)
            );         
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
            var commentVote = await cursor.FirstOrDefaultAsync(cancellationToken);
            if (commentVote == null)
                return defaultValue;
            return commentVote;
        }

        public async Task<List<T>> FindPostsCommentsVotesOwnedByAsync(string identityName,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.IdentityName, identityName);          
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
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
            var filter = Builders<T>.Filter.Eq(x => x._id, voteId);
            return await _collection.DeleteOneAsync(filter, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}