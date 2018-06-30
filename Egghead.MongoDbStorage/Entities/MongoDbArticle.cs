using System;
using Egghead.Common;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Entities
{
    public class MongoDbArticle
    {
        public MongoDbArticle()
        {
            Id = ObjectId.GenerateNewId().ToString();
            //Create indeces
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public ReleaseType ReleaseType { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Unlinkes { get; set; }
        public int Comments { get; set; }       
    }
}