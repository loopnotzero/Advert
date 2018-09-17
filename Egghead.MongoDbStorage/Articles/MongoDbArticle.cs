using System;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbArticle
    {
        public MongoDbArticle()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }

        public ObjectId Id { get; set; }
        
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        
        public string Text { get; set; }
        
        public string Email { get; set; }
        public string EmailNormalized { get; set; }
        
        public ReleaseType ReleaseType { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }       
    }
}