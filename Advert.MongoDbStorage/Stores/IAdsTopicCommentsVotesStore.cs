using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.AdsTopic;
using Advert.Common.AdsTopics;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public interface IAdsTopicCommentsVotesStore<T> : IDisposable where T : IAdsTopicCommentVote
    {
        Task CreateAdsTopicCommentVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdsTopicCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue, CancellationToken cancellationToken);
        Task<long> CountAdsTopicCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdsTopicCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}