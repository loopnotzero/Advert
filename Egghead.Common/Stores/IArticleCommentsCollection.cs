using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    public interface IArticleCommentsCollection<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<long> EstimatedArticleCommentsCountAsync(CancellationToken cancellationToken);
        Task<List<T>> FindArticleCommentsAsync(CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleCommentAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleCommentByIdAsync(ObjectId commentId, T entity, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleCommentByIdAsync(ObjectId commentId, CancellationToken cancellationToken);
    }
}