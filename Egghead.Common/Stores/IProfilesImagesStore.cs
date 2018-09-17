using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    public interface IProfilesImagesStore<T> : IDisposable where T : class
    {
        Task<T> GetProfileImageByProfileIdAsync(ObjectId profileId, CancellationToken cancellationToken);
        Task<OperationResult> CreateProfileImageAsync(T entity, CancellationToken cancellationToken);
    }
}