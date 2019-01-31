using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Common.Stores
{
    public interface IPostCommentsVotesStore<T> : IDisposable where T : IPostCommentVote
    {
        Task CreatePostCommentVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindPostCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue, CancellationToken cancellationToken);
        Task<List<T>> FindPostsCommentsVotesAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<long> CountPostCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<DeleteResult> DeletePostCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}