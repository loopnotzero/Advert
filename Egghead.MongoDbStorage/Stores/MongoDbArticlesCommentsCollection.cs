using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticleCommentsCollection<T> : IArticleCommentsCollection<T> where T : MongoDbArticleComment
    {
        private readonly IMongoCollection<T> _collection;
        
        public MongoDbArticleCommentsCollection(IMongoCollection<T> collection)
        {
            _collection = collection;
        }
        
        public async Task<T> FindArticleCommentById(string commentId, CancellationToken cancellationToken)
        {
            if (commentId == null)
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> EstimatedArticleCommentsCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<T>> GetArticleComments(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var findOptions = new FindOptions<T> {Sort = Builders<T>.Sort.Descending(field => field.CreatedAt)};

            var cursor = await _collection.FindAsync(Builders<T>.Filter.Empty, findOptions, cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<OperationResult> CreateArticleCommentAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateArticleCommentByIdAsync(string commentId, T entity, CancellationToken cancellationToken)
        {
            if (commentId == null)
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), entity, new UpdateOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> DeleteArticleCommentByIdAsync(string commentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, commentId), cancellationToken);

            return OperationResult.Success;
        }

        public void Dispose()
        {
        }
    }
}