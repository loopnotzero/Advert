using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.Managers
{
    public class ArticlesViewCountManager<T> : IDisposable where T : class
    {
        private bool _disposed;
        
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IArticlesViewCountStore<T> Store { get; set; }
        
        public ArticlesViewCountManager(IArticlesViewCountStore<T> store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }
        

        public async Task<long> CountArticleViewsCountAsync(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.CountArticleViewsCountByArticleIdAsync(articleId, CancellationToken);
        }
                
        public async Task<OperationResult> CreateArticleViewCountAsync(T entity)
        {
            ThrowIfDisposed();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await Store.CreateArticleViewsCountAsync(entity, CancellationToken);
        }
        
        public async Task<IQueryable<T>> AsQueryable()
        {
            ThrowIfDisposed();            
            return await Store.AsQueryable(CancellationToken);
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