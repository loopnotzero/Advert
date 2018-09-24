using System;
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
        Task<T> FindArticleCommentVoteAsync(ObjectId commentId, CancellationToken cancellationToken);
        Task<long> CountArticleCommentVotesAsync(ObjectId commentId, VoteType voteType, CancellationToken cancellationToken);
        Task<UpdateResult> UpdateArticleCommentVoteAsync(ObjectId voteId, VoteType voteType, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteArticleCommentVoteAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}