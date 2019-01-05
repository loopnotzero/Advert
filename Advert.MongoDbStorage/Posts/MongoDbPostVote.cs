using System;
using Advert.Common.Posts;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.Posts
{
    public class MongoDbPostVote : IPostVote
    {
        public MongoDbPostVote()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }
        public ObjectId Id { get; set; }  
        public ObjectId PostId { get; set; }
        public ObjectId ProfileId { get; set; }
        public VoteType VoteType { get; set; }   
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}