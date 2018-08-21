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
        public string ByWho { get; set; }
        public string ByWhoNormalized { get; set; }
        public ObjectId ArticleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UdpatedAt { get; set; }
    }
}