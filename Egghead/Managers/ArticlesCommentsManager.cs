using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.Models.Articles;
using Egghead.MongoDbStorage.Articles;
using MongoDB.Bson;

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
            
        public async Task<T> FindArticleCommentById(ObjectId articleId, ObjectId commendId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
            
            if (commendId == null)
            {
                throw new ArgumentNullException(nameof(commendId));
            }

            return await Store.GetArticleCommentsCollection(articleId.ToString(), CancellationToken).FindArticleCommentByIdAsync(commendId, CancellationToken);
        }
        
        public async Task<long> CountArticleCommentsByArticleIdAsync(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.GetArticleCommentsCollection(articleId.ToString(), CancellationToken).EstimatedArticleCommentsCountAsync(CancellationToken);
        }
        
        public async Task<List<T>> FindArticleCommentsByArticleId(ObjectId articleId)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            return await Store.GetArticleCommentsCollection(articleId.ToString(), CancellationToken).FindArticleCommentsAsync(CancellationToken);
        }

        public async Task<OperationResult> CreateArticleComment(ObjectId articleId, T entity)
        {
            ThrowIfDisposed();

            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var articleComments = Store.GetArticleCommentsCollection(articleId.ToString(), CancellationToken);

            return await articleComments.CreateArticleCommentAsync(entity, CancellationToken);
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