using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    public interface IProfilesStore<T> : IDisposable where T : class
    {
        Task<T> FindProfileByIdAsync(ObjectId id, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedEmailAsync(string email, CancellationToken cancellationToken);
        Task<OperationResult> CreateProfileAsync(T entity, CancellationToken cancellationToken);
    }
}