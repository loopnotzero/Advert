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
        public string NormalizedEmail { get; set; }
        public ReleaseType ReleaseType { get; set; }
        public double LikesCount { get; set; }
        public double DislikesCount { get; set; }
        public double ViewsCount { get; set; }
        public double CommentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }       
    }
}