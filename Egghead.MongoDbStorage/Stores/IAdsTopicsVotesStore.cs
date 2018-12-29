using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.AdsTopics;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IAdsTopicsVotesStore<T> : IDisposable where T : class
    {
        Task CreateAdsTopicVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdsTopicVoteByNormalizedEmailAsync(ObjectId adsId, string email, CancellationToken cancellationToken);
        Task<long> CountAdsTopicVotesByVoteTypeAsync(ObjectId adsId, VoteType voteType, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdsTopicVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}