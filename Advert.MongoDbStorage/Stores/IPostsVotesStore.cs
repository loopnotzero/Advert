using System;
using System.Collections.Generic;
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
        Task<T> FindPostVoteAsync(ObjectId postId, ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindPostVotesAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<long> CountPostVotesAsync(ObjectId postId, VoteType voteType, CancellationToken cancellationToken);
        Task<DeleteResult> DeletePostVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}