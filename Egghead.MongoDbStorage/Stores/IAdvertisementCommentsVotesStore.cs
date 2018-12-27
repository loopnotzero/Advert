using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Advertisements;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IAdvertisementCommentsVotesStore<T> : IDisposable where T : class
    {
        Task CreateAdvertisementCommentVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdvertisementCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue, CancellationToken cancellationToken);
        Task<long> CountAdvertisementCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdvertisementCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}