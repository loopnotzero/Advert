using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IProfilesPhotosStore<T> : IDisposable where T : class
    {
        Task CreateProfilePhotoAsync(T entity, CancellationToken cancellationToken);
        Task<T> GetProfilePhotoById(ObjectId imageId, CancellationToken cancellationToken);
        Task<T> GetProfilePhotoByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
    }
}