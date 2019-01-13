using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.MongoDbStorage.Stores;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.Managers
{
    public class PostCommentsManager<T> : IDisposable where T : IPostComment
    {
        private bool _disposed;
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IPostsCommentsStore<T> Store { get; set; }
        
        public PostCommentsManager(IPostsCommentsStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        
        public async Task CreatePostComment(string collectionName, T entity)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var postComments = Store.GetPostCommentsCollection(collectionName, CancellationToken);

            await postComments.CreatePostCommentAsync(entity, CancellationToken);
        }

        public async Task<T> FindPostComment(string collectionName, ObjectId commentId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }
            
            if (commentId.Equals(ObjectId.Empty))
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            return await Store.GetPostCommentsCollection(collectionName, CancellationToken).FindPostCommentByIdAsync(commentId, CancellationToken);
        }
       
        public async Task<long> CountPostCommentsAsync(string collectionName)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetPostCommentsCollection(collectionName, CancellationToken).CountPostCommentsCountAsync(CancellationToken);
        }

        public async Task<List<T>> FindPostCommentsAsync(string collectionName, int? limit)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetPostCommentsCollection(collectionName, CancellationToken).FindPostCommentsAsync(limit, CancellationToken);
        }
        
        public async Task<List<T>> FindPostCommentsAsync(string collectionName, int offset, int? limit)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetPostCommentsCollection(collectionName, CancellationToken).FindPostCommentsAsync(offset, limit, CancellationToken);
        }
        
        public async Task<List<T>> FindPostCommentsAsync(string collectionName, int offset, int? limit, SortDefinition sortDef)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            return await Store.GetPostCommentsCollection(collectionName, CancellationToken).FindPostCommentsAsync(offset, limit, sortDef, CancellationToken);
        }
        
        public async Task<List<T>> FindPostCommentsByProfileIdAsync(string collectionName, ObjectId profileId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            if (profileId.Equals(ObjectId.Empty))
            {
                throw new ArgumentNullException(nameof(profileId)); 
            }

            return await Store.GetPostCommentsCollection(collectionName, CancellationToken).FindPostCommentsByProfileIdAsync(profileId, CancellationToken);
        }
        
        public async Task<UpdateResult> DeletePostCommentAsync(string collectionName, ObjectId commentId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            if (commentId.Equals(ObjectId.Empty))
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            var postCommentsCollection = Store.GetPostCommentsCollection(collectionName, CancellationToken);

            return await postCommentsCollection.DeletePostCommentByIdAsync(commentId, CancellationToken);
        }
        
        public async Task<ReplaceOneResult> ReplacePostCommentAsync(string collectionName, ObjectId commentId, T entity)
        {
            ThrowIfDisposed();          
            var postCommentsCollection = Store.GetPostCommentsCollection(collectionName, CancellationToken);
            return await postCommentsCollection.ReplacePostCommentAsync(commentId, entity, CancellationToken);
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