using System;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbArticleComment
    {
        public MongoDbArticleComment()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        
        public long VotingPoints { get; set; }
        public string Text { get; set; }  
        public string ProfileName { get; set; }
        public string ProfilePhoto { get; set; }
        public ObjectId Id { get; set; }
        public ObjectId ReplyTo { get; set; }
        public ObjectId ArticleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}