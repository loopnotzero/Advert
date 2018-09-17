using System;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbArticleViewCount
    {
        public MongoDbArticleViewCount()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }

        public ObjectId Id { get; set; }
        
        public string Email { get; set; }
        public string EmailNormalized { get; set; }
        
        public ObjectId ArticleId { get; set; }
        
        public DateTime CreatedAt { get; set; }      
        public DateTime UdpatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}