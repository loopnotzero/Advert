using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Bazaar.Normalizers;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Services
{
    public class PostsVotesService<T> : IDisposable where T : IPostVote
    {
        private bool _disposed;

        /// <summary>
        /// The <see cref="T:Microsoft.AspNetCore.Identity.ILookupNormalizer" /> used to normalize things like user and role names.
        /// </summary>
        private ILookupNormalizer _keyNormalizer { get; set; }
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IPostsVotesStore<T> Store { get; set; }

        public PostsVotesService(IPostsVotesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            _keyNormalizer = keyNormalizer;
        }

        public async Task CreatePostVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreatePostVoteAsync(entity, CancellationToken);
        }

        public async Task<T> FindPostVoteByPostIdOwnedByAsync(ObjectId postId, string identityName)
        {
            ThrowIfDisposed();

            if (postId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(postId));
            }

            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentNullException(nameof(identityName));
            }

            return await Store.FindPostVoteByPostIdOwnedByAsync(postId, _keyNormalizer.NormalizeKey(identityName), CancellationToken);
        }
        
        public async Task<List<T>> FindPostsVotesOwnedByAsync(string identityName)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentNullException(nameof(identityName));
            }

            return await Store.FindPostsVotesOwnedByAsync(_keyNormalizer.NormalizeKey(identityName), CancellationToken);
        }

        public async Task<long> CountPostVotesByIdAsync(ObjectId postId, VoteType voteType)
        {
            ThrowIfDisposed();

            if (postId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(postId));
            }

            return await Store.CountPostVotesAsync(postId, voteType, CancellationToken);
        }

        public async Task<DeleteResult> DeletePostVoteByIdAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.DeletePostVoteByIdAsync(voteId, CancellationToken);
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

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}