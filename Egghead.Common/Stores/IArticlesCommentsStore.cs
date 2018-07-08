using System;
using System.Threading;
using System.Threading.Tasks;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesCommentsStore<T> : IDisposable where T : class
    {
        OperationResult DeleteArticleCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IArticlesCommentsCollection<T> GetArticleCommentsCollection(string collectionName);
        IArticlesCommentsCollection<T> CreateArticleCommentsCollection(string collectionName, CancellationToken cancellationToken);
    }
}