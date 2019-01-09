using System;
using Advert.Common.Posts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Advert.MongoDbStorage.Posts
{
    public class MongoDbPostComment : IPostComment
    {
        public MongoDbPostComment()
        {
            _id = ObjectId.GenerateNewId();
            //Create indeces
        }

        [BsonId]
        public ObjectId _id { get; set; }

        public bool IsDeleted { get; set; }
        public long VotesCount { get; set; }
        public string Text { get; set; }  
        public string ProfileName { get; set; }
        public string ProfileImagePath { get; set; }
        public ObjectId ReplyTo { get; set; }
        public ObjectId ProfileId { get; set; }
        public ObjectId PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}