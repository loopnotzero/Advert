using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    public interface IArticlesCommentsCollection<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentById(string commentId, CancellationToken cancellationToken);
        Task<long> CountArticleComments(string commentId, CancellationToken cancellationToken);
        Task<List<T>> GetArticleComments(CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleCommentAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleCommentByIdAsync(string commentId, T entity, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleCommentByIdAsync(string commentId, CancellationToken cancellationToken);
    }
}