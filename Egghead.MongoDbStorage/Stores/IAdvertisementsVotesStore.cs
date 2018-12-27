using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Advertisements;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IAdvertisementsVotesStore<T> : IDisposable where T : class
    {
        Task CreateAdvertisementVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdvertisementVoteByNormalizedEmailAsync(ObjectId adsId, string email, CancellationToken cancellationToken);
        Task<long> CountAdvertisementVotesByVoteTypeAsync(ObjectId adsId, VoteType voteType, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdvertisementVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}