using System.Collections.Generic;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbArticleCommentVoteAggregation
    {
        public MongoDbArticleCommentVoteAggregation()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public long VotesCount { get; set; }
        public ObjectId Id { get; set; }
        public VoteType VoteType { get; set; }
    }
}