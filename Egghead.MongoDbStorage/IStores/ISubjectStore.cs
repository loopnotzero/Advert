using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Egghead.MongoDbStorage.IStores
{
    public interface ISubjectStore<T> : IDisposable where T : class
    {              
        Task SetNormalizedTitleAsync(T subject, string normalizedTitle, CancellationToken cancellationToken);
        Task<T> FindSubjectByIdAsync(string subjectId, CancellationToken cancellationToken);
        Task<T> FindSubjectByTitleAsync(string normalizedTitle, CancellationToken cancellationToken);
        Task<IdentityResult> CreateSubjectAsync(T subject, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateSubjectAsync(T subject, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteSubjectAsync(T subject, CancellationToken cancellationToken);
    }
}