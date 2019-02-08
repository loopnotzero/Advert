using System;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Profiles;
using MongoDB.Bson;

namespace Bazaar.Common.Stores
{
    public interface IProfilesPhotosStore<T> : IDisposable where T : IProfilePhoto
    {
        Task CreateProfilePhotoAsync(T entity, CancellationToken cancellationToken);
        Task<T> GetProfilePhotoById(ObjectId imageId, CancellationToken cancellationToken);
        Task<T> GetProfilePhotoByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
    }
}