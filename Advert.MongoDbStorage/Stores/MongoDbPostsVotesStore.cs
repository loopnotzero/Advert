using System.Threading;
using System.Threading.Tasks;
using Advert.Common;
using Advert.Common.Posts;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Common;
using Advert.MongoDbStorage.Mappings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbPostsVotesStore<T> : IPostsVotesStore<T> where T : MongoDbPostVote
    {
        private readonly IMongoCollection<T> _collection;
        
        public void Dispose()
        {
            
        }

        public MongoDbPostsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostsVotes);          
            //todo: Create indices
        }

        private MongoDbPostsVotesStore()
        {
            EntityMappings.EnsureMongoDbPostVoteConfigured();
        }

        public async Task CreatePostVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.NormalizedEmail = entity.NormalizedEmail ?? entity.Email.ToUpper();
            
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostVoteByNormalizedEmailAsync(ObjectId postId, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.PostId, postId), Builders<T>.Filter.Eq(x => x.NormalizedEmail, email));
            var cursor = await _collection.FindAsync(filter, new FindOptions<T>
            {
                
            }, cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountPostVotesByVoteTypeAsync(ObjectId postId, VoteType voteType, CancellationToken cancellationToken)
        {      
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(x => x.PostId, postId), Builders<T>.Filter.Eq(x => x.VoteType, voteType)
                );          
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }
       
        public async Task<DeleteResult> DeletePostVoteByIdAsync(ObjectId voteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x.Id, voteId), cancellationToken);            
        }
    }
}