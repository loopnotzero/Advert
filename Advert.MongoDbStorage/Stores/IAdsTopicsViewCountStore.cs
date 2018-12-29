using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IAdsTopicsViewCountStore<T> : IDisposable where T : class
    {
        Task CreateAdsTopicViewsCountAsync(T entity , CancellationToken cancellationToken);
        Task<T> FindAdsTopicViewsCountByIdAsync(ObjectId id, CancellationToken cancellationToken);
        Task<long> CountAdsTopicViewsCountByAdsIdAsync(ObjectId adsId, CancellationToken cancellationToken);
    }
}