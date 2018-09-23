using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    //todo: Move this interface to Common
    public interface IArticlesVotesStore<T> : IDisposable where T : class
    {
        Task<T> FindArticleVoteByNormalizedEmailAsync(ObjectId articleId, string email, CancellationToken cancellationToken);
        Task<long> CountArticleVotesByTypeAsync(ObjectId articleId, VoteType voteType, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleVoteAsync(T entity, CancellationToken cancellationToken);
    }
}