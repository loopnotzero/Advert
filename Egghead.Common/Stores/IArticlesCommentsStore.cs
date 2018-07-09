using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesCommentsStore<T> : IDisposable where T : class
    {
        OperationResult DeleteArticleCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IArticleCommentsCollection<T> GetArticleCommentsCollection(string collectionName);
        IArticleCommentsCollection<T> CreateArticleCommentsCollection(string collectionName, CancellationToken cancellationToken);
    }
}