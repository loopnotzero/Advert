using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IAdvertisementsStore<T> : IDisposable where T : class
    {      
        Task CreateAdvertisementAsync(T entity, CancellationToken cancellationToken);
        Task UpdateAdvertisementAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindAdvertisementByIdAsync(ObjectId adsId, CancellationToken cancellationToken);
        Task<T> FindAdvertisementByIdOrDefaultAsync(ObjectId adsId, T defaultValue, CancellationToken cancellationToken);    
        Task<long> EstimatedAdvertisementsCountAsync(CancellationToken cancellationToken);
        Task<long> CountAdvertisementsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementsAsync(int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementsAsync(int offset, int? howManyElements, CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementsByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<List<T>> FindAdvertisementsWhichContainsKeywordAsync(int offset, int? howManyElements, string keyword, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteAdvertisementByIdAsync(ObjectId adsId, CancellationToken cancellationToken);      
        Task<UpdateResult> UpdateAdvertisementViewsCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken);
        Task<UpdateResult> UpdateAdvertisementLikesCountByAdsIdAsync(ObjectId adsId, long count, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplaceAdvertisementAsync(T entity, CancellationToken cancellationToken);
    }
}