using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    public interface IArticleCommentsVotesStore<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentVoteAsync(ObjectId articleId, ObjectId commentId, CancellationToken cancellationToken);
        Task<long> CountArticleCommentVotesAsync(ObjectId articleId, ObjectId commentId, VoteType voteType, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleCommentVoteAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleCommentVoteAsync(ObjectId voteId, VoteType voteType, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleCommentVoteAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}