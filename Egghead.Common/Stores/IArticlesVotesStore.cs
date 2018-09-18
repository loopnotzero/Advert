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
        Task<T> FindArticleVoteByProfileIdAsync(ObjectId articleId, VoteType voteType, ObjectId profileId,
            CancellationToken cancellationToken);
        Task<long> CountArticleVotesAsync(ObjectId articleId, VoteType voteType, CancellationToken cancellationToken);
        Task<OperationResult> CreateArticleVoteAsync(T entity , CancellationToken cancellationToken);
    }
}