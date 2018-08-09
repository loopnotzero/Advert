using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    public interface IArticleCommentsCollection<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentByIdAsync(string commentId, CancellationToken cancellationToken);
        Task<long> EstimatedArticleCommentsCountAsync(CancellationToken cancellationToken);
        Task<List<T>> FindArticleCommentsAsync(CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleCommentAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleCommentByIdAsync(string commentId, T entity, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleCommentByIdAsync(string commentId, CancellationToken cancellationToken);
    }
}