using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IArticleCommentsVotesStore<T> : IDisposable where T : class
    {
        Task CreateArticleCommentVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindArticleCommentVoteOrDefaultAsync(ObjectId commentId, ObjectId profileId, T defaultValue, CancellationToken cancellationToken);
        Task<long> CountArticleCommentVotesByCommentIdAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteArticleCommentVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}