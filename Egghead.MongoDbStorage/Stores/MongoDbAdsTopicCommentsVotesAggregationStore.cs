using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.AdsTopics;
using Egghead.MongoDbStorage.AdsTopics;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbAdsTopicCommentsVotesAggregationStore<T, A> : IAdsTopicCommentsVotesAggregationStore<T, A> where T : MongoDbAdsTopicCommentVote, new() where A : MongoDbAdsTopicCommentVoteAggregation, new()
    {
        private readonly IMongoCollection<T> _collection;

        private MongoDbAdsTopicCommentsVotesAggregationStore()
        {
            EntityMappings.EnsureMongoDbAdsTopicCommentsVotesAggregationConfigured();
        }
        
        public MongoDbAdsTopicCommentsVotesAggregationStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.AdsTopicCommentsVotes);
            //todo: Create indices
        }

        public async Task<A> CountAdsTopicCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
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