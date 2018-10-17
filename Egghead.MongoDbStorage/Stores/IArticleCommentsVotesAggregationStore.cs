using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IArticleCommentsVotesAggregationStore<T, A> : IDisposable where T : class where A : class
    {
        Task<A> CountArticleCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken);
    }
}