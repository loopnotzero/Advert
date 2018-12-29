using System.Collections.Generic;
using Advert.Common.AdsTopics;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.AdsTopics
{
    public class MongoDbAdsTopicCommentVoteAggregation
    {
        public MongoDbAdsTopicCommentVoteAggregation()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public long VotesCount { get; set; }
        public ObjectId Id { get; set; }
        public VoteType VoteType { get; set; }
    }
}