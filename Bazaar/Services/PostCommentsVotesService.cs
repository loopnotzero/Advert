using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bazaar.Services
{
    public class PostCommentsVotesService<T> : IDisposable where T : IPostCommentVote
    {
        private bool _disposed;

        private ILookupNormalizer _keyNormalizer { get; set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IPostCommentsVotesStore<T> Store { get; set; }

        public PostCommentsVotesService(IPostCommentsVotesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            _keyNormalizer = keyNormalizer;
        }

        public async Task CreatePostCommentVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreatePostCommentVoteAsync(entity, CancellationToken);
        }

        public async Task<T> FindPostCommentVoteByCommentIdOwnedByOrDefaultAsync(ObjectId commentId, string identityName, T defaultValue)
        {
            ThrowIfDisposed();

            if (commentId.Equals(ObjectId.Empty))
            {
                throw new ArgumentException(nameof(commentId));
            }

            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentException(nameof(identityName));
            }

            return await Store.FindPostCommentVoteByCommentIdOwnedByOrDefaultAsync(commentId, NormalizeKey(identityName), defaultValue, CancellationToken);
        }

        public async Task<List<T>> FindPostsCommentsVotesOwnedByAsync(string identityName)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(identityName))
            {
                throw new ArgumentException(nameof(identityName));
            }

            return await Store.FindPostsCommentsVotesOwnedByAsync(NormalizeKey(identityName), CancellationToken);
        }

        public async Task<long> CountPostCommentVotesByCommentIdAsync(ObjectId commentId)
        {
            ThrowIfDisposed();
            return await Store.CountPostCommentVotesByCommentIdAsync(commentId, CancellationToken);
        }

        public async Task<DeleteResult> DeletePostCommentVoteByIdAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId.Equals(ObjectId.Empty))
            {
                throw new ArgumentNullException(nameof(voteId));
            }

            return await Store.DeletePostCommentVoteByIdAsync(voteId, CancellationToken);
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
        
        private string NormalizeKey(string key)
        {
            return _keyNormalizer != null ? _keyNormalizer.Normalize(key) : key;
        }
    }
}