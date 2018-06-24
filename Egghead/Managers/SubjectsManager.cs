using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.MongoDbStorage.IStores;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;

namespace Egghead.Managers
{
    public class SubjectsManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal ISubjectStore<T> Store { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public SubjectsManager(ISubjectStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task SetNormalizedTitleAsync(T subject, string normalizedTitle)
        {
            ThrowIfDisposed();

            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            await Store.SetNormalizedTitleAsync(subject, normalizedTitle, CancellationToken);
        }

        public async Task<T> FindSubjectByIdAsync(string subjectId)
        {
            ThrowIfDisposed();

            if (subjectId == null)
            {
                throw new ArgumentNullException(nameof(subjectId));
            }

            return await Store.FindSubjectByIdAsync(subjectId, CancellationToken);
        }

        public async Task<T> FindSubjectByTitleAsync(string normalizedTitle)
        {
            ThrowIfDisposed();

            if (normalizedTitle == null)
            {
                throw new ArgumentNullException(nameof(normalizedTitle));
            }

            return await Store.FindSubjectByTitleAsync(normalizedTitle, CancellationToken);
        }
        
        public async Task<List<T>> GetSubjects()
        {
            ThrowIfDisposed();          
            return await Store.GetSubjects(CancellationToken);
        }

        public async Task<IdentityResult> CreateSubjectAsync(T subject)
        {
            ThrowIfDisposed();

            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return await Store.CreateSubjectAsync(subject, CancellationToken);
        }

        public async Task<IdentityResult> UpdateSubjectAsync(T subject)
        {
            ThrowIfDisposed();

            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return await Store.UpdateSubjectAsync(subject, CancellationToken);
        }

        public async Task<IdentityResult> DeleteSubjectAsync(T subject)
        {
            ThrowIfDisposed();

            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return await Store.DeleteSubjectAsync(subject, CancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            Store.Dispose();

            _disposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }     
    }
}