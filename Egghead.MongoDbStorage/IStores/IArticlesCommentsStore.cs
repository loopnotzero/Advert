using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;

namespace Egghead.MongoDbStorage.IStores
{
    //todo: Move this interface to Common
    public interface IArticlesCommentsStore<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentById(string id, CancellationToken cancellationToken);
        
        Task<long> CountArticlesCommentsByArticleIdAsync(string id, CancellationToken cancellationToken);
        
        Task<OperationResult> CreateArticleCommentAsync(T entity, CancellationToken cancellationToken);          
    }
}