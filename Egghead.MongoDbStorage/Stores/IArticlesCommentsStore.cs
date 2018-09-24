using System;
using System.Threading;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesCommentsStore<T> : IDisposable where T : class
    {
        bool ArticleCommentsCollectionExists(string collectionName, CancellationToken cancellationToken); 
        void DeleteArticleCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IArticleCommentsCollection<T> GetArticleCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IArticleCommentsCollection<T> CreateArticleCommentsCollection(string collectionName, CancellationToken cancellationToken);
    }
}