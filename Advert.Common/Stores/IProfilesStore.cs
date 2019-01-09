using System;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.Stores
{
    public interface IProfilesStore<T> : IDisposable where T : IProfile
    {
        Task CreateProfileAsync(T entity, CancellationToken cancellationToken);
        Task UpdateProfileAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindProfileByIdAsync(ObjectId id, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedNameAsync(string name, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedEmailAsync(string email, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedNameOrDefaultAsync(string name, T defaultValue, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedEmailOrDefaultAsync(string email, T defaultValue, CancellationToken cancellationToken);
    }
}