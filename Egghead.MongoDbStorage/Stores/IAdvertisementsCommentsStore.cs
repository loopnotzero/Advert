using System;
using System.Threading;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IAdvertisementsCommentsStore<T> : IDisposable where T : class
    {
        bool AdvertisementCommentsCollectionExists(string collectionName, CancellationToken cancellationToken); 
        void DeleteAdvertisementCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IAdvertisementCommentsCollection<T> GetAdvertisementCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IAdvertisementCommentsCollection<T> CreateAdvertisementCommentsCollection(string collectionName, CancellationToken cancellationToken);
    }
}