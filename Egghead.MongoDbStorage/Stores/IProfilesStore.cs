using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IProfilesStore<T> : IDisposable where T : class
    {
        Task CreateProfileAsync(T entity, CancellationToken cancellationToken);
        Task UpdateProfileAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindProfileByIdAsync(ObjectId id, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedNameAsync(string name, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedEmailAsync(string email, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedNameOrDefaultAsync(string name, CancellationToken cancellationToken);
    }
}