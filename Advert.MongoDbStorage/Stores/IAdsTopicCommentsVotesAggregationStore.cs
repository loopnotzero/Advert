using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.Stores
{
    public interface IAdsTopicCommentsVotesAggregationStore<T, A> : IDisposable where T : class where A : class
    {
        Task<A> CountAdsTopicCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken);
    }
}