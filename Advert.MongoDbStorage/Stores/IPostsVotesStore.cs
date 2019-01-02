using System;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public interface IPostsVotesStore<T> : IDisposable where T : IPostVote
    {
        Task CreatePostVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindPostVoteByNormalizedEmailAsync(ObjectId postId, string email, CancellationToken cancellationToken);
        Task<long> CountPostVotesByVoteTypeAsync(ObjectId postId, VoteType voteType, CancellationToken cancellationToken);
        Task<DeleteResult> DeletePostVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}