using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IAdvertisementsViewCountStore<T> : IDisposable where T : class
    {
        Task CreateAdvertisementViewsCountAsync(T entity , CancellationToken cancellationToken);
        Task<T> FindAdvertisementViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken);
        Task<long> CountAdvertisementViewsCountByAdsIdAsync(ObjectId adsId, CancellationToken cancellationToken);
    }
}