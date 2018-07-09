using System;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbArticleLike
    {
        public MongoDbArticleLike()
        {
            Id = ObjectId.GenerateNewId().ToString();
            //Create indeces
        }

        public string Id { get; set; }    
        public string ByWho { get; set; }
        public string ByWhoNormalized { get; set; }
        public string ArticleId { get; set; }     
        public LikeType LikeType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}