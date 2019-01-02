using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbPostCommentsVotesAggregationStore<T, A> : IPostCommentsVotesAggregationStore<T, A> where T : MongoDbPostCommentVote, new() where A : MongoDbPostCommentVoteAggregation, new()
    {
        private readonly IMongoCollection<T> _collection;

        private MongoDbPostCommentsVotesAggregationStore()
        {
            EntityMappings.EnsureMongoDbPostCommentsVotesAggregationConfigured();
        }
        
        public MongoDbPostCommentsVotesAggregationStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostCommentsVotes);
            //todo: Create indices
        }

        public async Task<A> CountPostCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            var pipeline = new EmptyPipelineDefinition<T>();

            var groupBy = pipeline.Match(x => x.CommentId.Equals(commentId))
                .Group(x => x.VoteType, grouping => new
                {
                    VoteType = grouping.Key,
                    VotesCount = grouping.LongCount()
                })
                .Project(x => new A
                {
                    VoteType = x.VoteType,
                    VotesCount = x.VotesCount
                });

            var asyncCursor = await _collection.AggregateAsync(groupBy, cancellationToken: cancellationToken);

            var list = await asyncCursor.ToListAsync(cancellationToken);

            return await asyncCursor.FirstOrDefaultAsync(cancellationToken);
        }

        public void Dispose()
        {

        }      
    }
}