using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IProfilesImagesStore<T> : IDisposable where T : class
    {
        Task CreateProfileImageAsync(T entity, CancellationToken cancellationToken);
        Task<T> GetProfileImageByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
    }
}