using System;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Entities
{
    public class MongoDbArticleComment
    {
        public MongoDbArticleComment()
        {
            Id = ObjectId.GenerateNewId().ToString();
            //Create indeces
        }

        public string Id { get; set; }
        public string Text { get; set; }
        public string ByWho { get; set; }
        public string ByWhoNormalized { get; set; }
        public string ReplyTo { get; set; }
        public int Depth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}