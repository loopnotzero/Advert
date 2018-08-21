using System;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbArticleVote
    {
        public MongoDbArticleVote()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }

        public ObjectId Id { get; set; }    
        public string ByWho { get; set; }
        public string ByWhoNormalized { get; set; }
        public ObjectId ArticleId { get; set; }     
        public VoteType VoteType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}