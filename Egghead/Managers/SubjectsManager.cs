using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;

namespace Egghead.Managers
{
    public class SubjectsManager<T> : ISubjectStore<T> where T : class
    {
        private readonly ISubjectStore<T> _subjectStore;

        public SubjectsManager(ISubjectStore<T> subjectStore)
        {
            _subjectStore = subjectStore;
        }
        
        public async Task SetNormalizedTitleAsync(T subject, string normalizedTitle, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _subjectStore.SetNormalizedTitleAsync(subject, normalizedTitle, cancellationToken);
        }

        public Task<T> FindSubjectByIdAsync(string subjectId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindSubjectByTitleAsync(string title, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteSubjectAsync(T subject, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}