using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Common.Stores
{
    public interface IPostsVotesStore<T> : IDisposable where T : IPostVote
    {
        Task CreatePostVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindPostVoteByPostIdOwnedByAsync(ObjectId postId, string identityName, CancellationToken cancellationToken);
        Task<List<T>> FindPostsVotesOwnedByAsync(string identityName, CancellationToken cancellationToken);
        Task<long> CountPostVotesAsync(ObjectId postId, VoteType voteType, CancellationToken cancellationToken);
        Task<DeleteResult> DeletePostVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}