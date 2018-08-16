using System.Threading;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Common.Stores;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Common;
using Egghead.MongoDbStorage.Mappings;
using MongoDB.Driver;

namespace Egghead.MongoDbStorage.Stores
{
    public class MongoDbArticlesVotesStore<T> : IArticlesVotesStore<T> where T : MongoDbArticleVote
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbArticlesVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.ArticlesVotes);          
            //todo: Create indices
        }

        private MongoDbArticlesVotesStore()
        {
            EntityMappings.EnsureMongoDbArticleLikeConfigured();
        }

        public async Task<T> FindArticleVoteAsync(string articleId, VoteType voteType, string byWhoNormalized, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.ArticleId, articleId),
                Builders<T>.Filter.Eq(x => x.VoteType, voteType),
                Builders<T>.Filter.Eq(x => x.ByWhoNormalized, byWhoNormalized));
            var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountArticleVotesAsync(string articleId, VoteType voteType, CancellationToken cancellationToken)
        {      
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.ArticleId, articleId), Builders<T>.Filter.Eq(x => x.VoteType, voteType)
                );          
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }
       
        public async Task<OperationResult> CreateArticleVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
            return OperationResult.Success;
        }
    }
}