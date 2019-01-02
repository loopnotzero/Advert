using System.Collections.Generic;
using Advert.Common.Posts;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.Posts
{
    public class MongoDbPostCommentVoteAggregation : IPostCommentVoteAggregation
    {
        public MongoDbPostCommentVoteAggregation()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public long VotesCount { get; set; }
        public ObjectId Id { get; set; }
        public VoteType VoteType { get; set; }
    }
}