using System;
using System.Threading;
using Advert.Common.Posts;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    //todo: Move this interface to Common
    public interface IPostsCommentsStore<T> : IDisposable where T : IPostComment
    {
        bool PostCommentsCollectionExists(string collectionName, CancellationToken cancellationToken); 
        void DeletePostCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IPostCommentsCollection<T> GetPostCommentsCollection(string collectionName, CancellationToken cancellationToken);
        IPostCommentsCollection<T> CreatePostCommentsCollection(string collectionName, CancellationToken cancellationToken);
    }
}