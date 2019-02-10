using System;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Profiles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Common.Stores
{
    public interface IProfilesStore<T> : IDisposable where T : IProfile
    {
        Task CreateProfileAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindProfileByIdAsync(ObjectId profileId, CancellationToken cancellationToken); 
        Task<T> FindProfileByIdOrDefaultAsync(ObjectId profileId, CancellationToken cancellationToken); 
        Task<T> FindProfileByNormalizedName(string profileName, CancellationToken cancellationToken);
        Task<T> FindProfileByNormalizedNameOrDefault(string profileName, T defaultValue, CancellationToken cancellationToken);
        Task<T> FindProfileByIdentityNameAsync(string email, CancellationToken cancellationToken);
        Task<T> FindProfileByIdentityNameOrDefaultAsync(string email, T defaultValue,
            CancellationToken cancellationToken);
        Task<ReplaceOneResult> UpdateProfileAsync(T entity, CancellationToken cancellationToken);
    }
}