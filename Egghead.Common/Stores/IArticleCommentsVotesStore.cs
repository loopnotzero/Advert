using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;

namespace Egghead.Common.Stores
{
    public interface IArticleCommentsVotesStore<T> : IDisposable where T : class
    {
        Task<T> FindArticleCommentVoteAsync(string articleId, string commentId, CancellationToken cancellationToken);
        Task<long> CountArticleCommentVotesAsync(string articleId, string commentId, VoteType voteType, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleCommentVoteAsync(T entity, CancellationToken cancellationToken);
        Task<OperationResult> UpdateArticleCommentVoteAsync(string voteId, VoteType voteType, CancellationToken cancellationToken);
        Task<OperationResult> DeleteArticleCommentVoteAsync(string voteId, CancellationToken cancellationToken);
    }
}