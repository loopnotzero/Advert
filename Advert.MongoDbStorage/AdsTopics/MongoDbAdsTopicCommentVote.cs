using System;
using Advert.Common.AdsTopics;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.AdsTopics
{
    public class MongoDbAdsTopicCommentVote
    {
        public MongoDbAdsTopicCommentVote()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public ObjectId Id { get; set; }
        public ObjectId AdsId { get; set; }      
        public ObjectId CommentId { get; set; }  
        public ObjectId ProfileId { get; set; }
        public VoteType VoteType { get; set; }       
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}