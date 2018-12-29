using System;
using System.Threading;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IAdsTopicsCommentsStore<T> : IDisposable where T : class
    {
        bool AdsTopicCommentsCollectionExists(string collectionName, CancellationToken cancellationToken); 
        void DeleteAdsTopicCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IAdsTopicCommentsCollection<T> GetAdsTopicCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IAdsTopicCommentsCollection<T> CreateAdsTopicCommentsCollection(string collectionName, CancellationToken cancellationToken);
    }
}