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
        Task CreateProfileImageAsync(T entity, CancellationToken cancellationToken);
        Task<T> GetProfileImageById(ObjectId imageId, CancellationToken cancellationToken);
        Task<T> GetProfileImageByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
    }
}