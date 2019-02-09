using System;
using System.Threading;
using System.Threading.Tasks;
using Bazaar.Common.Posts;
using Bazaar.Common.Stores;
using MongoDB.Bson;

namespace Bazaar.Services
{
    public class PostsViewsCountService<T> : IDisposable where T : IPostViewsCount
    {
        private bool _disposed;

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IPostsViewCountStore<T> Store { get; set; }

        public PostsViewsCountService(IPostsViewCountStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task CreatePostViewsCountAsync(T entity)
        {
            ThrowIfDisposed();
            await Store.CreatePostViewsCountAsync(entity, CancellationToken);
        }

        public async Task<long> CountPostViewsCountAsync(ObjectId postId)
        {
            ThrowIfDisposed();

            if (postId == null)
            {
                throw new ArgumentNullException(nameof(postId));
            }

            return await Store.CountPostViewsCountByPostIdAsync(postId, CancellationToken);
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