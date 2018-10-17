using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MongoDbArticleCommentsVotesAggregationStore<T, A> : IArticleCommentsVotesAggregationStore<T, A> where T : MongoDbArticleCommentVote, new() where A : MongoDbArticleCommentVoteAggregation, new()
    {
        private readonly IMongoCollection<T> _collection;

        private MongoDbArticleCommentsVotesAggregationStore()
        {
            EntityMappings.EnsureMongoDbArticleCommentsVotesAggregationConfigured();
        }
        
        public MongoDbArticleCommentsVotesAggregationStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticleCommentsVotes);
            //todo: Create indices
        }

        public async Task<A> CountArticleCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken)
        {
            var pipeline = new EmptyPipelineDefinition<T>();

            var groupBy = pipeline
                .Match(x => x.CommentId.Equals(commentId))
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

            var list = await asyncCursor.ToListAsync(cancellationToken: cancellationToken);

            return await asyncCursor.FirstOrDefaultAsync(cancellationToken);
        }

        public void Dispose()
        {

        }      
    }
}