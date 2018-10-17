using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticleCommentsVotesStore<T> : IArticleCommentsVotesStore<T> where T : MongoDbArticleCommentVote
    {
        private readonly IMongoCollection<T> _collection;

        public void Dispose()
        {

        }

        public MongoDbArticleCommentsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticleCommentsVotes);
            //todo: Create indices
        }

        private MongoDbArticleCommentsVotesStore()
        {
            EntityMappings.EnsureMongoDbArticleCommentVoteConfigured();
        }

        public async Task CreateArticleCommentVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindArticleCommentVoteByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.CommentId, commentId)
            );
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticleCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.CommentId, commentId)
            );
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<DeleteResult> DeleteArticleCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.Id, voteId);
            return await _collection.DeleteOneAsync(filter, cancellationToken);
        }
        
        public async Task<UpdateResult> UpdateArticleCommentVoteTypeByIdAsync(ObjectId voteId, VoteType voteType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var filter = Builders<T>.Filter.Eq(x => x.Id, voteId);

            var update = Builders<T>.Update.Set(x => x.VoteType, voteType).Set(x => x.UpdatedAt, DateTime.UtcNow);

            return await _collection.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = false
            }, cancellationToken);
        }
    }
}