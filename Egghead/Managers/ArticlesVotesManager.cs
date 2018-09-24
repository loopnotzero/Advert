﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.MongoDbStorage.Stores;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class ArticlesLikesManager<T> : IDisposable where T : class
    {
        private bool _disposed;

        /// <summary>
        /// The <see cref="T:Microsoft.AspNetCore.Identity.ILookupNormalizer" /> used to normalize things like user and role names.
        /// </summary>
        private ILookupNormalizer KeyNormalizer { get; set; }
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesVotesStore<T> Store { get; set; }

        public ArticlesLikesManager(IArticlesVotesStore<T> store, ILookupNormalizer keyNormalizer)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            KeyNormalizer = keyNormalizer;
        }

        public async Task CreateArticleVoteAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Store.CreateArticleVoteAsync(entity, CancellationToken);
        }

        public async Task<T> FindArticleVoteByAsync(ObjectId articleId, string normalizedEmail)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            return await Store.FindArticleVoteByAsync(articleId, normalizedEmail, CancellationToken);
        }

        public async Task<long> CountArticleTypedVotesByArticleIdAsync(ObjectId articleId, VoteType voteType)
        {
            ThrowIfDisposed();

            if (articleId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticleTypedVotesByArticleIdAsync(articleId, voteType, CancellationToken);
        }

        public async Task<DeleteResult> DeleteArticleVoteByIdAsync(ObjectId voteId)
        {
            ThrowIfDisposed();

            if (voteId == ObjectId.Empty)
            {
                throw new ArgumentNullException(nameof(voteId));
            }
            
            return await Store.DeleteArticleVoteByIdAsync(voteId, CancellationToken);
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