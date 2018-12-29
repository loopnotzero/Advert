using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IAdsTopicsStore<T> : IDisposable where T : class
    {      
        Task CreateAdsTopicAsync(T entity, CancellationToken cancellationToken);
        Task UpdateAdsTopicAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdsTopicByIdAsync(ObjectId adsId, CancellationToken cancellationToken);
        Task<T> FindAdsTopicByIdOrDefaultAsync(ObjectId adsId, T defaultValue, CancellationToken cancellationToken);    
        Task<long> EstimatedAdsTopicsCountAsync(CancellationToken cancellationToken);
        Task<long> CountAdsTopicsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicsAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicsAsync(int offset, int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindAdsTopicsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdsTopicByIdAsync(ObjectId adsId, CancellationToken cancellationToken);      
        Task<UpdateResult> UpdateAdsTopicViewsCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken);
        Task<UpdateResult> UpdateAdsTopicLikesCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplaceAdsTopicAsync(T entity, CancellationToken cancellationToken);
    }
}