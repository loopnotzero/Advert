using System.Collections.Generic;
using Egghead.Common.Advertisements;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Advertisements
{
    public class MongoDbAdvertisementCommentVoteAggregation
    {
        public MongoDbAdvertisementCommentVoteAggregation()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public long VotesCount { get; set; }
        public ObjectId Id { get; set; }
        public VoteType VoteType { get; set; }
    }
}