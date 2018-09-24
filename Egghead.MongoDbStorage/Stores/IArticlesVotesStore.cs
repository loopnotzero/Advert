using System;
using System.Threading;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public interface IArticlesVotesStore<T> : IDisposable where T : class
    {
        Task CreateArticleVoteAsync(T entity, CancellationToken cancellationToken);
        Task<T> FindArticleVoteByAsync(ObjectId articleId, string email, CancellationToken cancellationToken);
        Task<long> CountArticleTypedVotesByArticleIdAsync(ObjectId articleId, VoteType voteType, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteArticleVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken);
    }
}