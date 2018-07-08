using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Stores;

namespace Egghead.Managers
{
    public class ArticlesCommentsManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesCommentsStore<T> Store { get; set; }
        
        public ArticlesCommentsManager(IArticlesCommentsStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
       
        public async Task<long> CountArticlesCommentsByArticleId(string articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.GetArticleCommentsCollection(articleId).CountArticleComments(articleId, CancellationToken);
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