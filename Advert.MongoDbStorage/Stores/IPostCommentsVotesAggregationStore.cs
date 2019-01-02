using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.Stores
{
    public interface IPostCommentsVotesAggregationStore<T, A> : IDisposable where T : class where A : class
    {
        Task<A> CountPostCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken);
    }
}