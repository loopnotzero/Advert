using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Advert.MongoDbStorage.Stores
{
    public class MongoDbPostsVotesStore<T> : IPostsVotesStore<T> where T : IPostVote
    {
        private readonly IMongoCollection<T> _collection;
       
        public MongoDbPostsVotesStore(IMongoDatabase mongoDatabase) : this()
        {
            _collection = mongoDatabase.GetCollection<T>(MongoDbCollections.PostsVotes);          
            //todo: Create indices
        }

        private MongoDbPostsVotesStore()
        {
//            BsonClassMap.RegisterClassMap<MongoDbPostVote>(bsonClassMap =>
//            {
//                bsonClassMap.AutoMap();
//                bsonClassMap.MapIdMember(x => x.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
//            });
        }

        public async Task CreatePostVoteAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _collection.InsertOneAsync(entity, new InsertOneOptions
            {
                BypassDocumentValidation = false
            }, cancellationToken);
        }

        public async Task<T> FindPostVoteAsync(ObjectId postId, ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq(x => x.PostId, postId), Builders<T>.Filter.Eq(x => x.ProfileId, profileId));
            var cursor = await _collection.FindAsync(filter, new FindOptions<T>
            {
                
            }, cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<T>> FindPostsVotesAsync(ObjectId profileId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<T>.Filter.Eq(x => x.ProfileId, profileId);
            var cursor = await _collection.FindAsync(filter, new FindOptions<T>
            {
                
            }, cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<long> CountPostVotesAsync(ObjectId postId, VoteType voteType, CancellationToken cancellationToken)
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
            return await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(x => x._id, voteId), cancellationToken);            
        }

        public void Dispose()
        {
        }
    }
}