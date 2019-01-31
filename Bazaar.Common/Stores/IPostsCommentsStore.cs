using System;
using System.Threading;
using Bazaar.Common.Posts;

namespace Bazaar.Common.Stores
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